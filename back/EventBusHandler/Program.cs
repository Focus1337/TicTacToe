using EventBusHandler.Data;
using EventBusHandler.HostedServices;
using EventBusHandler.Options;
using EventBusHandler.RabbitMq;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services
    .Configure<DbOptions>(
        builder.Configuration.GetSection(DbOptions.DbConfiguration))
    .Configure<RabbitOptions>(
        builder.Configuration.GetSection(RabbitOptions.RabbitConfiguration));

var rabbitOptions = builder.Configuration.GetSection(RabbitOptions.RabbitConfiguration).Get<RabbitOptions>();
var dbOptions = builder.Configuration.GetSection(DbOptions.DbConfiguration).Get<DbOptions>();

services.AddSingleton(new ConnectionFactory
{
    HostName = rabbitOptions!.HostName,
    // DispatchConsumersAsync = true
});

services.AddSingleton<GameUpdateConsumer>();
services.AddHostedService<GameUpdateBusHandler>();
services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbOptions!.ConnectionString));

var app = builder.Build();

app.Run();