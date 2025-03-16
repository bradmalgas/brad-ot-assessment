namespace OT.Assessment.App.Messaging.Interfaces;

public interface ICasinoWagerPublisher
{
    public Task PublishAsync(CasinoWagerEventDTO casinoWager);
}