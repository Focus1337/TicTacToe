using MessageReceiverAndDbWriter.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessageReceiverAndDbWriter.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    public DbSet<Message> Messages { get; set; } = null!;
}