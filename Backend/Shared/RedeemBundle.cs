namespace Backend.Shared;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a bundle of offline tokens to be redeemed on-chain.
/// </summary>
public class RedeemBundle
{
    /// <summary>
    /// Cosmos DB uses <c>id</c> as the primary key, keep the lower-case name
    /// through JSON mapping while exposing a PascalCase property.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("serials")]
    public string[] Serials { get; set; } = Array.Empty<string>();

    [JsonPropertyName("processed")]
    public bool Processed { get; set; } = false;
}

