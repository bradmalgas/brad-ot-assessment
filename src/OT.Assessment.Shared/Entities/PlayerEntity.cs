namespace OT.Assessment.Shared.Models;

public class PlayerEntity
{
    public Guid AccountId { get; set; }
    public string Username { get; set; }
    public decimal TotalSpend { get; set; } = 0;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CasinoWagerEntity> CasinoWagers { get; set; }
}