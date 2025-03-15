namespace OT.Assessment.Shared.Messaging.Implementation;

public class CasinoWagerPublisher : ICasinoWagerPublisher
{
    private readonly RabbitMqConfiguration _config;
    private readonly IRabbitMqChannelFactory _channelProvider;

    public CasinoWagerPublisher(IOptions<RabbitMqConfiguration> configuration, IRabbitMqChannelFactory channelProvider)
    {
        _config = configuration.Value;
        _channelProvider = channelProvider;
    }

    public async Task PublishAsync(CasinoWagerDTO casinoWager)
    {
        using var channel = await _channelProvider.CreateChannelAsync();
        var message = JsonConvert.SerializeObject(casinoWager);
        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: _config.ExchangeName, routingKey: _config.RoutingKey, body: body, basicProperties: properties, mandatory: true);
        Console.WriteLine($" [x] Sent {message}");
    }
}