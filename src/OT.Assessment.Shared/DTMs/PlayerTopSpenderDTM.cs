namespace OT.Assessment.Shared.Models;

public class PlayerTopSpenderDTM
{
    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("totalAmountSpend")]
    public decimal TotalAmountSpend { get; set; }
}