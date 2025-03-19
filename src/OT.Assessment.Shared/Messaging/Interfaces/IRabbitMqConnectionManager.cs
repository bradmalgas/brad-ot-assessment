namespace OT.Assessment.Shared.Messaging.Interfaces;

public interface IRabbitMqConnectionManager : IDisposable
{
    Task<IConnection> GetConnection();
}