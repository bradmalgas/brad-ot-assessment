namespace OT.Assessment.Shared.Messaging.Interfaces;

public interface ICasinoWagerPublisher
{
    Task PublishAsync(CasinoWagerDTO casinoWager);
}