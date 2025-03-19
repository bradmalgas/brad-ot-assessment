using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Messaging;
using OT.Assessment.Shared.Messaging.Interfaces;
using OT.Assessment.Shared.Models;
using RabbitMQ.Client;

namespace OT.Assessment.App.Services.Implementation;

public class CasinoWagerPublishService : ICasinoWagerPublishService
{
    private readonly ILogger<CasinoWagerPublishService> _logger;
    private readonly IRabbitMqChannelFactory _channelFactory;
    private readonly RabbitMqConfiguration _config;
    public CasinoWagerPublishService(ILogger<CasinoWagerPublishService> logger, IRabbitMqChannelFactory channelFactory, IOptions<RabbitMqConfiguration> configuration)
    {
        _logger = logger;
        _channelFactory = channelFactory;
        _config = configuration.Value;
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
        using var channel = await _channelFactory.CreateChannelAsync();
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));
        await channel.BasicPublishAsync(exchange: _config.ExchangeName, routingKey: _config.RoutingKey, body: body);
        _logger.LogInformation($" [x] Sent casino wager to queue [ID: {request.WagerId}]");
    }
}