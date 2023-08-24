using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite;
using Microsoft.EntityFrameworkCore;

namespace Telebox.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Connection> Connections { get; set; } = default!;

    public DbSet<Recording> Recordings { get; set; } = default!;

    public DbSet<Setting> Settings { get; set; } = default!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddQuartz(builder => builder.UseSqlite());
        
        base.OnModelCreating(modelBuilder);
    }
}