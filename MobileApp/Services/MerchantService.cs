namespace OfflineGuide.Services;

public class MerchantService
{
    public record PendingRedeem(decimal Amount, string UserHash);
    public List<PendingRedeem> PendingRedeems { get; } = new();

    public async Task SyncAsync()
    {
        // TODO: post bundle to Azure Function
        await Task.Delay(500);
        PendingRedeems.Clear();
    }
}