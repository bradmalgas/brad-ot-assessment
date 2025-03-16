using OT.Assessment.Shared.Data.Interfaces;

namespace OT.Assessment.Shared.Data.Implementation;

public class CasinoWagerRepository : ICasinoWagerRepository
{
    private readonly CasinoWagersDbContext _dbContext;
    public CasinoWagerRepository(CasinoWagersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddWagerAsync(CasinoWagerEntity wager)
    {
        wager.CreatedAt = DateTime.UtcNow;
        _dbContext.CasinoWagers.Add(wager);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateWagerAsync(CasinoWagerEntity wager)
    {
        _dbContext.CasinoWagers.Update(wager);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<CasinoWagerEntity?> GetWagerByIdAsync(Guid wagerId)
    {
        return await _dbContext.CasinoWagers.FindAsync(wagerId);
    }

    public async Task<PaginatedResult<CasinoWagerDTO>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page)
    {
        var query = _dbContext.CasinoWagers
            .Where(w => w.AccountId == playerId)
            .OrderByDescending(w => w.WagerDateTime);

        int totalRecords = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(w => new CasinoWagerDTO
            {
                WagerId = w.WagerId,
                Game = w.GameName,
                Provider = w.Provider,
                Amount = w.Amount,
                CreatedDate = w.WagerDateTime,
            })
            .ToListAsync();

        return new PaginatedResult<CasinoWagerDTO>
        {
            Data = data,
            Page = page,
            PageSize = pageSize,
            Total = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
        };
    }
}