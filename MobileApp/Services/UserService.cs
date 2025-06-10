using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace JPYCOffline.Services;

public class UserService
{
    readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7071/api/") };
    public string? Token { get; private set; }

    public async Task<bool> SignUpAsync(string username, string password)
    {
        var res = await _http.PostAsJsonAsync("signup", new { username, password });
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> SignInAsync(string username, string password)
    {
        var res = await _http.PostAsJsonAsync("signin", new { username, password });
        if (!res.IsSuccessStatusCode) return false;
        var json = await res.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Token = doc.RootElement.GetProperty("token").GetString();
        if (Token != null)
        {
            await SecureStorage.Default.SetAsync("auth_token", Token);
            return true;
        }
        return false;
    }
}
