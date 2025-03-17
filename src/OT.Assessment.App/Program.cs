using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OT.Assessment.Shared.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckl
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.EnableTryItOutByDefault();
        opts.DocumentTitle = "OT Assessment App";
        opts.DisplayRequestDuration();
    });
}

var hostName = builder.Configuration["RabbitMq:HostName"];
var userName = builder.Configuration["RabbitMq:UserName"];
var password = builder.Configuration["RabbitMq:Password"];
var queueName = builder.Configuration["RabbitMq:QueueName"];
var exchangeName = builder.Configuration["RabbitMq:ExchangeName"];

app.MapGet("/", () => "The API is running")
.WithName("Health Check");

//POST api/player/casinowager
app.MapPost("api/player/casinowager", async ([FromBody] CasinoWagerRequest request) =>
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
    var factory = new ConnectionFactory
    {
        HostName = hostName,
        UserName = userName,
        Password = password,
    };
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();
    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));
    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
})
.WithName("PublishCasinoWager")
.WithOpenApi();

//GET api/player/{playerId}/wagers
app.MapGet("api/player/{playerId}/wagers", ([FromRoute] Guid playerId) => "Get player wagers works");

//GET api/player/topSpenders?count=10
app.MapGet("api/player/topSpenders", ([FromQuery] int count) => "Get top spenders works");


app.Run();
