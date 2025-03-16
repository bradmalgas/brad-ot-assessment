using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OT.Assessment.Shared.Messaging;
using OT.Assessment.Shared.Messaging.Interfaces;
using RabbitMQ.Client;

namespace OT.Assessment.App.Messaging.Implementation;

public class CasinoWagerPublisher : ICasinoWagerPublisher
{
    private readonly RabbitMqConfiguration _config;
    private readonly IRabbitMqChannelFactory _channelProvider;

    public CasinoWagerPublisher(IOptions<RabbitMqConfiguration> configuration, IRabbitMqChannelFactory channelProvider)
    {
        _config = configuration.Value;
        _channelProvider = channelProvider;
    }

    public async Task PublishAsync(CasinoWagerEventDTO casinoWager)
    {
        using var channel = await _channelProvider.CreateChannelAsync();
        var message = JsonConvert.SerializeObject(casinoWager);
        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: _config.ExchangeName, routingKey: _config.RoutingKey, body: body, basicProperties: properties, mandatory: true);
        Console.WriteLine($" [x] Sent {message}");
    }
}