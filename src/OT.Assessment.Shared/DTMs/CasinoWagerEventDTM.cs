namespace OT.Assessment.Shared.Models;

public class CasinoWagerEventDTM
{
    [JsonProperty("wagerId")]
    public Guid WagerId { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }

    [JsonProperty("gameName")]
    public string GameName { get; set; }

    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; }
}