using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Azure.Storage.Queues;
using Microsoft.Azure.Cosmos;
using Backend.Shared;

namespace RedeemFunction;

public class RedeemOrchestrator
{
    private readonly ILogger _logger;
    private readonly Web3 _web3;
    private readonly CosmosClient _cosmos;
    private readonly QueueClient _queue;
    private readonly string _contract;
    private readonly string _db;
    private readonly string _container;

    public RedeemOrchestrator(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RedeemOrchestrator>();
        var rpc = Environment.GetEnvironmentVariable("RPC_URL") ?? "https://polygon-rpc.com";
        var pk = Environment.GetEnvironmentVariable("PRIVATE_KEY") ?? "";
        _web3 = string.IsNullOrEmpty(pk) ? new Web3(rpc) : new Web3(new Account(pk), rpc);
        _cosmos = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_CONNECTION")!);
        _queue = new QueueClient(
            Environment.GetEnvironmentVariable("QUEUE_CONNECTION"),
            Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "redeem-bundles");
        _contract = Environment.GetEnvironmentVariable("CONTRACT_ADDRESS") ?? "0x";
        _db = Environment.GetEnvironmentVariable("BUNDLE_DB_NAME") ?? "offline";
        _container = Environment.GetEnvironmentVariable("BUNDLE_CONTAINER_NAME") ?? "bundles";
    }

    [Function("RedeemExecutor")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timer)
    {
        _logger.LogInformation("RedeemExecutor fired at {time}", DateTime.UtcNow);

        if (!await _queue.ExistsAsync()) return;
        var msgs = await _queue.ReceiveMessagesAsync(32);
        foreach (var msg in msgs.Value)
        {
            var id = msg.Body.ToString();
            try
            {
                var container = _cosmos.GetContainer(_db, _container);
                var response = await container.ReadItemAsync<RedeemBundle>(id, new PartitionKey(id));
                var bundle = response.Resource;
                if (bundle.processed)
                {
                    await _queue.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
                    continue;
                }

                var abi = "[{\"inputs\":[{\"internalType\":\"bytes[]\",\"name\":\"serials\",\"type\":\"bytes[]\"}],\"name\":\"redeemBundle\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
                var contract = _web3.Eth.GetContract(abi, _contract);
                var func = contract.GetFunction("redeemBundle");
                await func.SendTransactionAsync(_web3.TransactionManager.Account?.Address, new object[] { bundle.serials });

                bundle.processed = true;
                await container.ReplaceItemAsync(bundle, bundle.id);
                await _queue.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process bundle {id}", id);
            }
        }
    }
}
