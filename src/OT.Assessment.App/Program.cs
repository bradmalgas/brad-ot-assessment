using System.Reflection;
using Microsoft.AspNetCore.Mvc;

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

app.MapGet("/", () => "The API is running")
.WithName("Health Check");

//POST api/player/casinowager
app.MapPost("api/player/casinowager", () => "Post wager works");

//GET api/player/{playerId}/wagers
app.MapGet("api/player/{playerId}/wagers", ([FromRoute] Guid playerId) => "Get player wagers works");

//GET api/player/topSpenders?count=10
app.MapGet("api/player/topSpenders", ([FromQuery] int count) => "Get top spenders works");


app.Run();
