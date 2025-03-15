using OT.Assessment.Shared.Data.Interfaces;

namespace OT.Assessment.Shared.Data.Implementation;

public class CasinoWagerRepository : ICasinoWagerRepository
{
    private readonly CasinoWagersDbContext _dbContext;
    private readonly IPlayersRepository _playersRepository;
    public CasinoWagerRepository(CasinoWagersDbContext dbContext, IPlayersRepository playersRepository)
    {
        _dbContext = dbContext;
        _playersRepository = playersRepository;
    }

    public async Task AddWagerAsync(CasinoWagerEntity wager, PlayerEntity player)
    {
        var exisitngPlayer = await _playersRepository.GetPlayerByIdAsync(player.AccountId);

        if (exisitngPlayer == null)
        {
            await _playersRepository.AddPlayerAsync(player);
        }
        wager.CreatedAt = DateTime.UtcNow;
        _dbContext.CasinoWagers.Add(wager);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedResult<CasinoWagerEntity>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page)
    {
        var query = _dbContext.CasinoWagers
            .Where(w => w.AccountId == playerId)
            .OrderByDescending(w => w.WagerDateTime);

        int totalRecords = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<CasinoWagerEntity>
        {
            Data = data,
            Page = page,
            PageSize = pageSize,
            Total = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
        };
    }

    public async Task<List<PlayerEntity>> GetTopSpendersAsync(int count)
    {
        return await _dbContext.CasinoWagers
        .GroupBy(w => w.AccountId)
        .OrderByDescending(t => t.Sum(w => w.Amount))
        .Take(count)
        .Select(g => _dbContext.Players.Find(g.Key))
        .ToListAsync();
    }
}