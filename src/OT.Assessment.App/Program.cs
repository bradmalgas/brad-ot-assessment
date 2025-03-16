using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OT.Assessment.App.Messaging.Implementation;
using OT.Assessment.App.Services.Implementation;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Data.Implementation;
using OT.Assessment.Shared.Data.Interfaces;
using OT.Assessment.Shared.Messaging;
using OT.Assessment.Shared.Messaging.Implementation;
using OT.Assessment.Shared.Messaging.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContext<CasinoWagersDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("OT.Assessment.Consumer")));
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<ICasinoWagerApiService, CasinoWagerApiService>();
builder.Services.AddSingleton<ICasinoWagerPublisher, CasinoWagerPublisher>();
builder.Services.AddSingleton<IRabbitMqChannelFactory, RabbitMqChannelFactory>();
builder.Services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
    builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }
    );
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5021, options => options.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2);
    serverOptions.ListenLocalhost(7120, options =>
    {
        options.UseHttps();
    });
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

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();