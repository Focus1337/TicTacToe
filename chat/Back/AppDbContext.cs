using Back.Entities;
using Microsoft.EntityFrameworkCore;

namespace Back;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=database;Port=5432;Username=testuser;Password=testpass;Database=messagesdb;");
    }

    public DbSet<Message> Messages { get; set; } = null!;
}