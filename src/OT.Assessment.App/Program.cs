using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OT.Assessment.App.Services.Implementation;
using OT.Assessment.App.Services.Interfaces;
using OT.Assessment.Shared.Data.Implementation;
using OT.Assessment.Shared.Data.Interfaces;
using OT.Assessment.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckl
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddDbContext<CasinoWagersDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("OT.Assessment.Consumer")));
builder.Services.AddScoped<ICasinoWagerPublishService, CasinoWagerPublishService>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();
builder.Services.AddScoped<ICasinoWagerApiService, CasinoWagerApiService>();

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
app.MapPost("api/player/casinowager", async ([FromBody] CasinoWagerRequest request, ICasinoWagerPublishService publishService) =>
{
    await publishService.PublishAsync(request);
})
.WithName("PublishCasinoWager")
.WithOpenApi();

//GET api/player/{playerId}/wagers
app.MapGet("api/player/{playerId}/wagers", async ([FromRoute] Guid playerId, [FromQuery] int pageSize, [FromQuery] int page, ICasinoWagerApiService apiService) =>
{
    return await apiService.GetWagersByPlayerAsync(playerId: playerId, pageSize: pageSize, page: page);
});

//GET api/player/topSpenders?count=10
app.MapGet("api/player/topSpenders", async ([FromQuery] int count, ICasinoWagerApiService apiService) =>
{
    return await apiService.GetTopSpendersAsync(count);
});


app.Run();
