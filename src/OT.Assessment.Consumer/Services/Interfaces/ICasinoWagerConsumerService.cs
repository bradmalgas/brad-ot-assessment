namespace OT.Assessment.Consumer.Services.Interfaces;

public interface ICasinoWagerConsumerService : IDisposable
{
    Task AddWagerAsync(CasinoWagerEventDTM dto);
}