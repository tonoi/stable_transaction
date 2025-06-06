using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace RedeemFunction;

public class RedeemOrchestrator
{
    private readonly ILogger _logger;
    private readonly Web3 _web3 = new("https://polygon-rpc.com");

    public RedeemOrchestrator(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RedeemOrchestrator>();
    }

    [Function("RedeemExecutor")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timer)
    {
        _logger.LogInformation("RedeemExecutor fired at {time}", DateTime.UtcNow);
        // TODO: fetch pending bundles, call smart contract
    }
}