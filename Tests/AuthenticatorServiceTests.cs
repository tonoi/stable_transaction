using System.Threading.Tasks;
using JPYCOffline.Services;
using Xunit;

public class AuthenticatorServiceTests
{
    [Fact]
    public async Task AddGenerateAndRestore()
    {
        var store = new InMemoryStore();
        var service = new AuthenticatorService(store);
        await service.AddAccountAsync("Test", "alice", "JBSWY3DPEHPK3PXP");
        await service.AddAccountAsync("Test", "bob", "JBSWY3DPEHPK3PXP");

        var code = await service.GenerateTotpAsync("Test", "alice");
        Assert.Equal(6, code.Length);

        var service2 = new AuthenticatorService(store);
        var accounts = await service2.GetAccountsAsync();
        Assert.Equal(2, accounts.Count);
    }

    class InMemoryStore : IKeyValueStore
    {
        readonly Dictionary<string,string> _dict = new();
        public Task SetAsync(string key, string value){_dict[key]=value;return Task.CompletedTask;}
        public Task<string?> GetAsync(string key)=>Task.FromResult(_dict.TryGetValue(key,out var v)?v:null);
        public Task RemoveAsync(string key){_dict.Remove(key);return Task.CompletedTask;}
    }
}
