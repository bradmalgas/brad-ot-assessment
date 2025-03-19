namespace OT.Assessment.Shared.Messaging.Implementation;

public class RabbitMqConnectionManager : IRabbitMqConnectionManager
{
    private readonly RabbitMqConfiguration _config;
    private IConnection? _connection;
    public RabbitMqConnectionManager(IOptions<RabbitMqConfiguration> configuration)
    {
        _config = configuration.Value;
    }

    private async Task InitializeConnectionAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _config.HostName,
            UserName = _config.UserName,
            Password = _config.Password
        };

        _connection = await factory.CreateConnectionAsync();
    }

    public async Task<IConnection> GetConnection()
    {
        if (_connection == null)
        {
            await InitializeConnectionAsync();
        }
        return _connection;
    }

    public void Dispose()
    {
        _connection?.CloseAsync();
        GC.SuppressFinalize(this);
    }
}