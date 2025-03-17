namespace OT.Assessment.Shared.Models;

public class CasinoWagerRequest
{
    [JsonProperty("wagerId")]
    public Guid WagerId { get; set; }

    [JsonProperty("theme")]
    public string Theme { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }

    [JsonProperty("gameName")]
    public string GameName { get; set; }

    [JsonProperty("transactionId")]
    public string TransactionId { get; set; }

    [JsonProperty("brandId")]
    public string BrandId { get; set; }

    [JsonProperty("accountId")]
    public Guid AccountId { get; set; }

    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("externalReferenceId")]
    public string ExternalReferenceId { get; set; }

    [JsonProperty("transactionTypeId")]
    public string TransactionTypeId { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("createdDateTime")]
    public DateTime CreatedDateTime { get; set; }

    [JsonProperty("numberOfBets")]
    public int NumberOfBets { get; set; }

    [JsonProperty("countryCode")]
    public string CountryCode { get; set; }

    [JsonProperty("sessionData")]
    public string SessionData { get; set; }

    [JsonProperty("Duration")]
    public long Duration { get; set; }
}