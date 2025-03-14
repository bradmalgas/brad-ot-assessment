namespace OT.Assessment.Shared.Models;

public class CasinoWagerEntity
{
    public Guid WagerId { get; set; }
    public Guid AccountId { get; set; }
    public string GameName { get; set; }
    public string Provider { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDateTime { get; set; }
}