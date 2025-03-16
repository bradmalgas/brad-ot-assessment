namespace OT.Assessment.Shared.Data.Interfaces;

public interface ICasinoWagerRepository
{
    Task AddWagerAsync(CasinoWagerEntity wager);
    Task<PaginatedResult<CasinoWagerDTO>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page);
    Task UpdateWagerAsync(CasinoWagerEntity wager);
    Task<CasinoWagerEntity?> GetWagerByIdAsync(Guid wagerId);

}