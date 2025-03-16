using OT.Assessment.Shared.Data.Interfaces;

namespace OT.Assessment.Shared.Data.Implementation;

public class PlayersRepository : IPlayersRepository
{
    private readonly CasinoWagersDbContext _dbContext;
    public PlayersRepository(CasinoWagersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddPlayerAsync(PlayerEntity player)
    {
        player.CreatedAt = DateTime.UtcNow;
        _dbContext.Players.Add(player);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PlayerEntity?> GetPlayerByIdAsync(Guid playerId)
    {
        return await _dbContext.Players.FindAsync(playerId);
    }

    public async Task<List<PlayerTopSpenderDTM>> GetTopSpendersAsync(int count)
    {
        return await _dbContext.Players
            .Select(p => new PlayerTopSpenderDTM
            {
                AccountId = p.AccountId,
                Username = p.Username,
                TotalAmountSpend = _dbContext.CasinoWagers
                    .Where(w => w.AccountId == p.AccountId)
                    .Sum(w => w.Amount)
            })
            .OrderByDescending(p => p.TotalAmountSpend)
            .Take(count)
            .ToListAsync();
    }
}