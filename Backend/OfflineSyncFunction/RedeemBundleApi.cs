using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace OfflineSyncFunction;

public class RedeemBundleApi
{
    private readonly ILogger _logger;
    public RedeemBundleApi(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RedeemBundleApi>();
    }

    [Function("RedeemBundle")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
                                            FunctionContext executionContext)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Received redeem bundle");
        // TODO: validate JWT, write to Cosmos, enqueue durable orchestration
        var res = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
        await res.WriteStringAsync("{"status":"accepted"}");
        return res;
    }
}