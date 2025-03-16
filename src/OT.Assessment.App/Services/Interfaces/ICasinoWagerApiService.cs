namespace OT.Assessment.App.Services.Interfaces;

public interface ICasinoWagerApiService
{
    Task PublishAsync(CasinoWagerEventDTO casinoWager);
    Task<PaginatedResult<CasinoWagerDTO>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page);
    Task<List<PlayerTopSpenderDTM>> GetTopSpendersAsync(int count);
}