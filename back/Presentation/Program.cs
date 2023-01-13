using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using Presentation.Context;
using Presentation.Entities;
using Presentation.RabbitMq;
using Presentation.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<GameUpdateProducer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => options.ClaimsIssuer = JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Auth/Login");
    });

builder.Services.AddAuthorization();
builder.Services.AddDbContext<PostgresDbContext>(options =>
    {
        options.UseNpgsql("Host=postgres;Port=5432;Username=testuser;Password=testpass;Database=testDb;",
            action => action.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        options.UseOpenIddict();
    })
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<PostgresDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
});
builder.Services.AddOpenIddict()
    .AddCore(options =>
        options.UseEntityFrameworkCore()
            .UseDbContext<PostgresDbContext>())
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

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetService<PostgresDbContext>();
    await context!.Database.MigrateAsync();
}

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