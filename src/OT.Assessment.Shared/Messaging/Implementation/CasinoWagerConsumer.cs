using RabbitMQ.Client.Events;

namespace OT.Assessment.Shared.Messaging.Implementation;

public class CasinoWagerConsumer : ICasinoWagerConsumer
{
    private readonly RabbitMqConfiguration _config;
    private readonly IRabbitMqChannelFactory _channelProvider;

    public CasinoWagerConsumer(IOptions<RabbitMqConfiguration> configuration, IRabbitMqChannelFactory channelProvider)
    {
        _config = configuration.Value;
        _channelProvider = channelProvider;
    }

    public async Task ConsumeAsync()
    {
        Console.WriteLine(" [*] Waiting for messages.");
        using var channel = await _channelProvider.CreateChannelAsync();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
            var wager = JsonConvert.DeserializeObject<CasinoWagerDTO>(message);

            try
            {
                // Write to SQL database
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                Console.WriteLine(" [x] Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(queue: _config.QueueName, autoAck: false, consumer: consumer);

        await Task.Delay(Timeout.Infinite);
    }
}