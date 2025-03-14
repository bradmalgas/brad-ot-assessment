namespace OT.Assessment.Shared.Models;

public class PlayerTopSpenderDTM
{
    [JsonPropertyName("accountId")]
    public Guid AccountId { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("totalAmountSpend")]
    public decimal TotalAmountSpend { get; set; }
}