using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OT.Assessment.Shared.Data.Interfaces;

public interface ICasinoWagersDbContext : IDisposable
{
    DbSet<CasinoWagerEntity> CasinoWagers { get; set; }
    DbSet<PlayerEntity> Players { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DatabaseFacade Database { get; }
}