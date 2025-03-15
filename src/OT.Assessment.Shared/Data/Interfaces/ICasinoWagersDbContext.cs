namespace OT.Assessment.Shared.Data.Interfaces;

public interface ICasinoWagersDbContext
{
    DbSet<CasinoWagerEntity> CasinoWagers { get; set; }
    DbSet<PlayerEntity> Players { get; set; }
}