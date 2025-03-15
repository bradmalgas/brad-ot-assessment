namespace OT.Assessment.Shared.Data.Interfaces;

public interface ICasinoWagerRepository
{
    Task AddWagerAsync(CasinoWagerEntity wager, PlayerEntity player);
    Task<PaginatedResult<CasinoWagerEntity>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page);
    Task<List<PlayerEntity>> GetTopSpendersAsync(int count);
}