using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using OT.Assessment.App.Services.Implementation;
using OT.Assessment.App.Services.Interfaces;
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

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["RabbitMq:HostName"],
        UserName = builder.Configuration["RabbitMq:UserName"],
        Password = builder.Configuration["RabbitMq:Password"],
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});
builder.Services.AddScoped<ICasinoWagerPublishService, CasinoWagerPublishService>();

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

app.MapGet("/", () => "The API is running")
.WithName("Health Check");

//POST api/player/casinowager
app.MapPost("api/player/casinowager", async ([FromBody] CasinoWagerRequest request, ICasinoWagerPublishService apiService) =>
{
    await apiService.PublishAsync(request);
})
.WithName("PublishCasinoWager")
.WithOpenApi();

//GET api/player/{playerId}/wagers
app.MapGet("api/player/{playerId}/wagers", ([FromRoute] Guid playerId) => "Get player wagers works");

//GET api/player/topSpenders?count=10
app.MapGet("api/player/topSpenders", ([FromQuery] int count) => "Get top spenders works");


app.Run();
