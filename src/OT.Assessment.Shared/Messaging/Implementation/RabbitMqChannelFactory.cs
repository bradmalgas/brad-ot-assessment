namespace OT.Assessment.Shared.Messaging.Implementation;

public class RabbitMqChannelFactory : IRabbitMqChannelFactory
{
    private readonly IRabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqConfiguration _config;

    public RabbitMqChannelFactory(IOptions<RabbitMqConfiguration> configuration, IRabbitMqConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
        _config = configuration.Value;
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        IConnection connection = _connectionManager.GetConnection();
        IChannel channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _config.ExchangeName, type: ExchangeType.Topic);
        await channel.QueueDeclareAsync(queue: _config.QueueName, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(queue: _config.QueueName, exchange: _config.ExchangeName, routingKey: _config.RoutingKey);

        return channel;
    }
}