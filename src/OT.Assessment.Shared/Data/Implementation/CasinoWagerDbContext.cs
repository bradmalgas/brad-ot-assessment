using OT.Assessment.Shared.Data.Interfaces;

namespace OT.Assessment.Shared.Data.Implementation;

public class CasinoWagersDbContext : DbContext, ICasinoWagersDbContext
{
    public CasinoWagersDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<CasinoWagerEntity> CasinoWagers { get; set; }
    public DbSet<PlayerEntity> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CasinoWagerEntity>()
        .HasKey(w => w.WagerId);

        modelBuilder.Entity<CasinoWagerEntity>()
        .HasIndex(w => w.AccountId);

        modelBuilder.Entity<CasinoWagerEntity>()
        .HasIndex(w => w.WagerDateTime);

        modelBuilder.Entity<CasinoWagerEntity>()
        .HasIndex(w => w.Amount);

        modelBuilder.Entity<CasinoWagerEntity>()
        .HasIndex(w => new { w.AccountId, w.WagerDateTime });

        modelBuilder.Entity<CasinoWagerEntity>()
        .HasOne(w => w.Player)
        .WithMany(p => p.CasinoWagers)
        .HasForeignKey(w => w.AccountId);

        modelBuilder.Entity<PlayerEntity>()
        .HasKey(p => p.AccountId);
    }
}
