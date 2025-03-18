using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OT.Assessment.Consumer.Interfaces;
using OT.Assessment.Consumer.Messaging.Implementation;
using OT.Assessment.Consumer.Services.Implementation;
using OT.Assessment.Shared.Data.Implementation;
using OT.Assessment.Shared.Data.Interfaces;
using OT.Assessment.Shared.Messaging;
using OT.Assessment.Shared.Messaging.Implementation;
using OT.Assessment.Shared.Messaging.Interfaces;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        //configure services

        services.AddDbContext<CasinoWagersDbContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("OT.Assessment.Consumer")));
        services.Configure<RabbitMqConfiguration>(context.Configuration.GetSection("RabbitMq"));
        services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();
        services.AddScoped<IPlayersRepository, PlayersRepository>();
        services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();
        services.AddSingleton<IRabbitMqChannelFactory, RabbitMqChannelFactory>();
        services.AddScoped<ICasinoWagerConsumerService, CasinoWagerService>();
        services.AddSingleton<ICasinoWagerConsumer, CasinoWagerConsumer>();
        services.AddScoped<ICasinoWagersDbContext, CasinoWagersDbContext>();
        services.AddSingleton<ICasinoWagerConsumerServiceFactory, CasinoWagerConsumerServiceFactory>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

var consumer = host.Services.GetService<ICasinoWagerConsumer>();
await consumer.ConsumeAsync();

await host.RunAsync();

logger.LogInformation("Application ended {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);