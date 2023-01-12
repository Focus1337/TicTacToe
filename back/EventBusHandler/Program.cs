using EventBusHandler.HostedServices;
using EventBusHandler.RabbitMq;
using MongoDbContext = EventBusHandler.Data.MongoDbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<GameUpdateConsumer>();
builder.Services.AddHostedService<GameUpdateBusHandler>();

var app = builder.Build();

app.Run();