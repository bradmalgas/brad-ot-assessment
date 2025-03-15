namespace OT.Assessment.Shared.Models;

public class CasinoWagerDTO
{
    [JsonProperty("wagerId")]
    public Guid WagerId { get; set; }
    [JsonProperty("game")]
    public string Game { get; set; }
    [JsonProperty("provider")]
    public string Provider { get; set; }
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}