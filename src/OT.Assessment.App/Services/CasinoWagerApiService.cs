using System.Text;
using System.Text.Json;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Data.Interfaces;
using OT.Assessment.Shared.Models;
using RabbitMQ.Client;

namespace OT.Assessment.App.Services.Implementation;

public class CasinoWagerApiService : ICasinoWagerApiService
{
    private readonly ICasinoWagerRepository _wagerRepository;
    private readonly IPlayersRepository _playersRepository;
    public CasinoWagerApiService(ICasinoWagerRepository wagerRepository, IPlayersRepository playersRepository)
    {
        _wagerRepository = wagerRepository;
        _playersRepository = playersRepository;
    }

    public async Task<List<PlayerTopSpenderDTM>> GetTopSpendersAsync(int count)
    {
        return await _playersRepository.GetTopSpendersAsync(count);
    }

    public async Task<PaginatedResult<CasinoWagerDTO>> GetWagersByPlayerAsync(Guid playerId, int pageSize, int page)
    {
        return await _wagerRepository.GetWagersByPlayerAsync(playerId: playerId, page: page, pageSize: pageSize);
    }
}