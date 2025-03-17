using System.Text;
using System.Text.Json;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Models;
using RabbitMQ.Client;

namespace OT.Assessment.App.Services.Implementation;

public class CasinoWagerPublishService : ICasinoWagerPublishService
{
    private readonly string _queueName;
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly ILogger<CasinoWagerPublishService> _logger;
    public CasinoWagerPublishService(IConfiguration configuration, ILogger<CasinoWagerPublishService> logger)
    {
        _queueName = configuration["RabbitMq:QueueName"];
        _hostName = configuration["RabbitMq:HostName"];
        _userName = configuration["RabbitMq:UserName"];
        _password = configuration["RabbitMq:Password"];
        _logger = logger;
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
        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password,
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueName, body: body);
        _logger.LogInformation($" [x] Sent casino wager to queue [ID: {request.WagerId}]");
    }
}