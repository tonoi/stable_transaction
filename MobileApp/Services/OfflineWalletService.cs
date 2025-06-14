namespace OfflineGuide.Services;

public class OfflineWalletService
{
    public decimal Balance { get; private set; } = 0m;

    public Task<bool> SendOfflineAsync(string merchantPubKey, decimal amount)
    {
        // TODO: create OPT, store in SE, transmit over BLE
        Balance -= amount;
        return Task.FromResult(true);
    }
}