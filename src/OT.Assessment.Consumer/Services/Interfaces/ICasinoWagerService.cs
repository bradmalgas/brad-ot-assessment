namespace OT.Assessment.Consumer.Services.Interfaces;

public interface ICasinoWagerService : IDisposable
{
    Task AddWagerAsync(CasinoWagerEventDTO dto);
}