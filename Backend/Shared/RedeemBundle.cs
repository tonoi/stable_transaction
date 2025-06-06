namespace Backend.Shared;

public class RedeemBundle
{
    // Cosmos DB uses 'id' as the primary key
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string[] serials { get; set; } = Array.Empty<string>();
    public bool processed { get; set; } = false;
}

