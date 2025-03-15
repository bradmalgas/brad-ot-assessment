namespace OT.Assessment.Shared.Messaging.Interfaces;

public interface IRabbitMqChannelFactory
{
    Task<IChannel> CreateChannelAsync();
}