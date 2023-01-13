using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presentation.Entities;

namespace Presentation.Context;

public class PostgresDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }

    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     base.OnModelCreating(builder);
    //     // builder
    //     //     .CreateUsers();
    // }
}