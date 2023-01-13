using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Presentation.Data;
using Presentation.Entities;
using Presentation.RabbitMq;
using Presentation.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<GameUpdateProducer>();
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => options.ClaimsIssuer = JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();
builder.Services.AddDbContext<DbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DbConfiguration"),
            action => action.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        options.UseOpenIddict();
    })
    .AddIdentity<User, IdentityRole>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<DbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
});
builder.Services.AddOpenIddict()
    .AddCore(options =>
        options.UseEntityFrameworkCore()
            .UseDbContext<DbContext>())
    .AddServer(options =>
    {
        options
            .AcceptAnonymousClients()
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow();
        options.SetTokenEndpointUris("/Auth/Login");
        var cfg = options.UseAspNetCore();
        if (builder.Environment.IsDevelopment())
            cfg.DisableTransportSecurityRequirement();
        cfg.EnableTokenEndpointPassthrough();
        options
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
    })
    .AddValidation(options =>
    {
        options.UseAspNetCore();
        options.UseLocalServer();
    });

builder.Services.AddSignalR(opt => opt.EnableDetailedErrors = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost").AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("/game");

app.Run();