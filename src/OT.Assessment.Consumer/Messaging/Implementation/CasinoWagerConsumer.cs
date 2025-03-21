using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OT.Assessment.Consumer.Services.Implementation;
using OT.Assessment.Shared.Consumer.Interfaces;
using OT.Assessment.Shared.Messaging;
using OT.Assessment.Shared.Messaging.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OT.Assessment.Consumer.Messaging.Implementation;

public class CasinoWagerConsumer : ICasinoWagerConsumer
{
    private readonly RabbitMqConfiguration _config;
    private readonly IRabbitMqChannelFactory _channelProvider;
    private readonly ICasinoWagerServiceFactory _casinoWagerServiceFactory;
    private readonly ILogger<CasinoWagerConsumer> _logger;

    public CasinoWagerConsumer(IOptions<RabbitMqConfiguration> configuration, IRabbitMqChannelFactory channelProvider, ICasinoWagerServiceFactory casinoWagerServiceFactory, ILogger<CasinoWagerConsumer> logger)
    {
        _config = configuration.Value;
        _channelProvider = channelProvider;
        _casinoWagerServiceFactory = casinoWagerServiceFactory;
        _logger = logger;
    }

    public async Task ConsumeAsync()
    {
        _logger.LogInformation(" [*] Waiting for messages.");
        using var channel = await _channelProvider.CreateChannelAsync();
        using var casinoWagerService = _casinoWagerServiceFactory.CreateService();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($" [x] Received {message}");
            var wager = JsonConvert.DeserializeObject<CasinoWagerEventDTM>(message);

            try
            {
                await casinoWagerService.AddWagerAsync(wager);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                _logger.LogInformation(" [x] Done");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error processing message: {ex.Message}");
                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(queue: _config.QueueName, autoAck: false, consumer: consumer);

        await Task.Delay(Timeout.Infinite);
    }
}