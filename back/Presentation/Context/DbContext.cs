using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presentation.Entities;

namespace Presentation.Context;

public class DbContext : IdentityDbContext<User>
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    { }

    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     base.OnModelCreating(builder);
    //     // builder
    //     //     .CreateUsers();
    // }
}