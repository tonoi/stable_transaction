using System.Text.Json.Serialization;

namespace Backend.Shared;

public class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
