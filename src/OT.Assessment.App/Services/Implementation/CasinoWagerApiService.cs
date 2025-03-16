using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Data.Interfaces;

namespace OT.Assessment.App.Services.Implementation;

public class CasinoWagerApiService : ICasinoWagerApiService
{
    private readonly ICasinoWagerPublisher _casinoWagerPublisher;
    private readonly ICasinoWagerRepository _wagerRepository;
    private readonly IPlayersRepository _playersRepository;

    public CasinoWagerApiService(ICasinoWagerPublisher casinoWagerPublisher, ICasinoWagerRepository wagerRepository, IPlayersRepository playersRepository)
    {
        _casinoWagerPublisher = casinoWagerPublisher;
        _wagerRepository = wagerRepository;
        _playersRepository = playersRepository;
    }

    public Task<List<PlayerTopSpenderDTM>> GetTopSpendersAsync(int count)
    {
        return _playersRepository.GetTopSpendersAsync(count);
    }

    public Task<PaginatedResult<CasinoWagerDTO>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page)
    {
        return _wagerRepository.GetWagersByPlayerAsync(playerId: playerId, page: page, pageSize: pageSize);
    }

    public Task PublishAsync(CasinoWagerEventDTO casinoWager)
    {
        return _casinoWagerPublisher.PublishAsync(casinoWager);
    }
}