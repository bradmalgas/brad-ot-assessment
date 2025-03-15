using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OT.Assessment.Shared.Data.Implementation;
using OT.Assessment.Shared.Data.Interfaces;

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
        services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();
        services.AddScoped<IPlayersRepository, PlayersRepository>();

    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

await host.RunAsync();

logger.LogInformation("Application ended {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);