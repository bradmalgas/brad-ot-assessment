using System.Text;
using System.Text.Json;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Models;
using RabbitMQ.Client;

namespace OT.Assessment.App.Services.Implementation;

public class CasinoWagerPublishService : ICasinoWagerPublishService
{
    private readonly IConnection _connection;
    private readonly string _queueName;
    public CasinoWagerPublishService(IConnection connection, IConfiguration configuration)
    {
        _connection = connection;
        _queueName = configuration["RabbitMq:QueueName"];
    }

    public async Task PublishAsync(CasinoWagerRequest request)
    {
        var dto = new CasinoWagerEventDTM
        {
            WagerId = request.WagerId,
            GameName = request.GameName,
            Provider = request.Provider,
            Amount = request.Amount,
            CreatedDateTime = request.CreatedDateTime,
            AccountId = request.AccountId,
            Username = request.Username
        };
        using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueName, body: body);
    }
}