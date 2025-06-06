using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Queues;
using Microsoft.IdentityModel.Tokens;
using Backend.Shared;

namespace OfflineSyncFunction;

public class RedeemBundleApi
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmos;
    private readonly QueueClient _queue;
    private readonly TokenValidationParameters _tokenParams;

    public RedeemBundleApi(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RedeemBundleApi>();
        _cosmos = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_CONNECTION")!);
        _queue = new QueueClient(
            Environment.GetEnvironmentVariable("QUEUE_CONNECTION"),
            Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "redeem-bundles");
        _queue.CreateIfNotExists();

        var key = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? "secret";
        _tokenParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    }

    [Function("RedeemBundle")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
                                            FunctionContext executionContext)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Received redeem bundle");

        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        var bearer = authHeaders.FirstOrDefault()?.Split(' ').Last();
        if (string.IsNullOrEmpty(bearer))
            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);

        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(bearer, _tokenParams, out _);
        }
        catch
        {
            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        }

        var bundle = JsonSerializer.Deserialize<RedeemBundle>(body);
        if (bundle == null)
            return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);

        var db = Environment.GetEnvironmentVariable("BUNDLE_DB_NAME") ?? "offline";
        var containerName = Environment.GetEnvironmentVariable("BUNDLE_CONTAINER_NAME") ?? "bundles";
        var container = _cosmos.GetContainer(db, containerName);
        await container.CreateItemAsync(bundle);

        await _queue.SendMessageAsync(bundle.id);

        var res = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
        await res.WriteStringAsync("{\"status\":\"accepted\"}");
        return res;
    }
}
