namespace OT.Assessment.Shared.Messaging.Interfaces;

public interface IRabbitMqConnectionManager : IDisposable
{
    IConnection GetConnection();
}