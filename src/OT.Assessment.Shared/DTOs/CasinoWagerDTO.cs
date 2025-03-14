namespace OT.Assessment.Shared.Models;

public class CasinoWagerDTO
{
    [JsonPropertyName("wagerId")]
    public Guid WagerId { get; set; }
    [JsonPropertyName("game")]
    public string Game { get; set; }
    [JsonPropertyName("provider")]
    public string Provider { get; set; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }
}