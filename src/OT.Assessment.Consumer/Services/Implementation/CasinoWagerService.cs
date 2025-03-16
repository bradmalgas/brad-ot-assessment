using Microsoft.EntityFrameworkCore;
using OT.Assessment.Shared.Data.Interfaces;

namespace OT.Assessment.Consumer.Services.Implementation;

public class CasinoWagerService : ICasinoWagerService
{
    private readonly ICasinoWagerRepository _wagerRepository;
    private readonly IPlayersRepository _playerRepository;
    private readonly ICasinoWagersDbContext _dbContext;
    private readonly ILogger _logger;

    public CasinoWagerService(ICasinoWagerRepository wagerRepository, IPlayersRepository playerRepository, ICasinoWagersDbContext context, ILogger<CasinoWagerService> logger)
    {
        _wagerRepository = wagerRepository;
        _playerRepository = playerRepository;
        _dbContext = context;
        _logger = logger;
    }

    public async Task AddWagerAsync(CasinoWagerEventDTO dto)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {

            _logger.LogInformation($"[{dto.WagerId}] Remove wager from queue");
            try
            {
                _logger.LogInformation($"[{dto.WagerId}] Checking if player exists");
                var existingPlayer = await _playerRepository.GetPlayerByIdAsync(dto.AccountId);

                if (existingPlayer == null)
                {
                    var newPlayer = new PlayerEntity
                    {
                        AccountId = dto.AccountId,
                        Username = dto.Username
                    };

                    _logger.LogInformation($"[{dto.WagerId}] Player does not exist, creating new player (ID: {dto.AccountId})");
                    await _playerRepository.AddPlayerAsync(newPlayer);
                }

                _logger.LogInformation($"[{dto.WagerId}] Checking if wager exists");
                var existingWager = await _wagerRepository.GetWagerByIdAsync(dto.WagerId);

                if (existingWager != null)
                {
                    existingWager.WagerId = dto.WagerId;
                    existingWager.AccountId = dto.AccountId;
                    existingWager.GameName = dto.GameName;
                    existingWager.Provider = dto.Provider;
                    existingWager.Amount = dto.Amount;
                    existingWager.WagerDateTime = dto.CreatedDateTime;

                    _logger.LogInformation($"[{dto.WagerId}] Wager DOES exist, **updating** exisitng wager (ID: {dto.WagerId})");
                    await _wagerRepository.UpdateWagerAsync(existingWager);
                }
                else
                {

                    var wager = new CasinoWagerEntity
                    {
                        WagerId = dto.WagerId,
                        AccountId = dto.AccountId,
                        GameName = dto.GameName,
                        Provider = dto.Provider,
                        Amount = dto.Amount,
                        WagerDateTime = dto.CreatedDateTime
                    };
                    _logger.LogInformation($"[{dto.WagerId}] Wager does not exist, **creating** new wager (ID: {dto.WagerId})");
                    await _wagerRepository.AddWagerAsync(wager);

                }
                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Concurrency error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                Console.WriteLine($"Error processing wager: {ex.Message}");
                throw;
            }
        }

    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}