using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Backend.Shared;

namespace OfflineSyncFunction;

public class AuthApi
{
    readonly ILogger _logger;
    readonly CosmosClient _cosmos;
    readonly PasswordHasher<User> _hasher = new();
    readonly string _db;
    readonly string _container;
    readonly string _jwtKey;

    public AuthApi(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AuthApi>();
        _cosmos = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_CONNECTION")!);
        _db = Environment.GetEnvironmentVariable("BUNDLE_DB_NAME") ?? "offline";
        _container = Environment.GetEnvironmentVariable("USER_CONTAINER_NAME") ?? "users";
        _jwtKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? "secret";
    }

    record Credentials(string username, string password);

    [Function("SignUp")]
    public async Task<HttpResponseData> SignUp([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var cred = await JsonSerializer.DeserializeAsync<Credentials>(req.Body);
        if (cred == null || string.IsNullOrEmpty(cred.username) || string.IsNullOrEmpty(cred.password))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        var container = _cosmos.GetContainer(_db, _container);
        var user = new User { Id = cred.username };
        user.PasswordHash = _hasher.HashPassword(user, cred.password);
        await container.CreateItemAsync(user, new PartitionKey(user.Id));
        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function("SignIn")]
    public async Task<HttpResponseData> SignIn([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var cred = await JsonSerializer.DeserializeAsync<Credentials>(req.Body);
        if (cred == null)
            return req.CreateResponse(HttpStatusCode.BadRequest);

        var container = _cosmos.GetContainer(_db, _container);
        try
        {
            var response = await container.ReadItemAsync<User>(cred.username, new PartitionKey(cred.username));
            var user = response.Resource;
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, cred.password);
            if (result == PasswordVerificationResult.Failed)
                return req.CreateResponse(HttpStatusCode.Unauthorized);

            var claims = new[] { new Claim("sub", cred.username) };
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: creds, expires: DateTime.UtcNow.AddHours(12));
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteStringAsync(JsonSerializer.Serialize(new { token = tokenStr }));
            return res;
        }
        catch (CosmosException)
        {
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}
