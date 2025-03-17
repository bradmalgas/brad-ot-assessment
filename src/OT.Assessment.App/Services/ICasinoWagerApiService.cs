using OT.Assessment.Shared.Models;

namespace OT.Assessment.App.Services.Interfaces;

public interface ICasinoWagerApiService
{
    Task<PaginatedResult<CasinoWagerDTO>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page);
    Task<List<PlayerTopSpenderDTM>> GetTopSpendersAsync(int count);
}