namespace OT.Assessment.Shared.Data.Interfaces;

public interface IPlayersRepository
{
    Task<PlayerEntity?> GetPlayerByIdAsync(Guid playerId);
    Task AddPlayerAsync(PlayerEntity player);
    Task<List<PlayerTopSpenderDTM>> GetTopSpendersAsync(int count);
}