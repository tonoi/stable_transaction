namespace OfflineGuide.Services.Travel;

public class WalletAccount
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Icon { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}

public interface IWalletService
{
    Task<List<WalletAccount>> GetAccountsAsync();
    Task<WalletAccount?> GetAccountByIdAsync(string id);
    Task<bool> ConnectAccountAsync(string accountType);
    Task<bool> DisconnectAccountAsync(string accountId);
    Task<PaymentResult> ProcessPaymentAsync(string accountId, decimal amount, string description);
    Task<decimal> GetAccountBalanceAsync(string accountId);
}

public class WalletService : IWalletService
{
    private readonly List<WalletAccount> _accounts = new()
    {
        new WalletAccount
        {
            Id = "1",
            Name = "My Number Card",
            Type = "MyNumber",
            Balance = 15000,
            Icon = "credit-card",
            IsConnected = true,
            LastUpdated = DateTime.Now.AddMinutes(-15)
        },
        new WalletAccount
        {
            Id = "2",
            Name = "Suica",
            Type = "Transit",
            Balance = 2500,
            Icon = "train",
            IsConnected = true,
            LastUpdated = DateTime.Now.AddHours(-2)
        },
        new WalletAccount
        {
            Id = "3",
            Name = "Google Pay",
            Type = "Digital",
            Balance = 50000,
            Icon = "google",
            IsConnected = false,
            LastUpdated = DateTime.Now.AddDays(-1)
        },
        new WalletAccount
        {
            Id = "4",
            Name = "Microsoft Account",
            Type = "Digital",
            Balance = 0,
            Icon = "microsoft",
            IsConnected = false,
            LastUpdated = DateTime.Now.AddDays(-3)
        }
    };

    public async Task<List<WalletAccount>> GetAccountsAsync()
    {
        await Task.Delay(300); // Simulate network call
        return _accounts.ToList();
    }

    public async Task<WalletAccount?> GetAccountByIdAsync(string id)
    {
        await Task.Delay(200); // Simulate network call
        return _accounts.FirstOrDefault(a => a.Id == id);
    }

    public async Task<bool> ConnectAccountAsync(string accountType)
    {
        await Task.Delay(1000); // Simulate connection process

        var account = _accounts.FirstOrDefault(a => a.Type == accountType && !a.IsConnected);
        if (account != null)
        {
            account.IsConnected = true;
            return true;
        }

        return false;
    }

    public async Task<bool> DisconnectAccountAsync(string accountId)
    {
        await Task.Delay(500); // Simulate disconnection process

        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        if (account != null)
        {
            account.IsConnected = false;
            return true;
        }

        return false;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(string accountId, decimal amount, string description)
    {
        await Task.Delay(2000); // Simulate payment processing

        var account = _accounts.FirstOrDefault(a => a.Id == accountId && a.IsConnected);
        if (account == null)
        {
            return new PaymentResult
            {
                Success = false,
                Message = "Account not found or not connected"
            };
        }

        if (account.Balance < amount)
        {
            return new PaymentResult
            {
                Success = false,
                Message = "Insufficient balance"
            };
        }

        account.Balance -= amount;
        return new PaymentResult
        {
            Success = true,
            Message = "Payment successful",
            TransactionId = Guid.NewGuid().ToString()
        };
    }

    public async Task<decimal> GetAccountBalanceAsync(string accountId)
    {
        await Task.Delay(200); // Simulate network call

        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        return account?.Balance ?? 0;
    }
}
