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

    Task<List<PlayerTopSpenderDTM>> IPlayersRepository.GetTopSpendersAsync(int count)
    {
        return _dbContext.Players
        // .OrderByDescending(p => p.TotalSpend)
        // .Take(count)
        .Select(p => new PlayerTopSpenderDTM
        {
            AccountId = p.AccountId,
            Username = p.Username,
        })
        .ToListAsync();
    }
}