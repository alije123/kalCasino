using kalCasino.Models;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Database;

public sealed class DataContext : DbContext
{
    public DataContext()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }

    public DbSet<Shop> Shop { get; set; }
    public DbSet<LuckHour> LuckHour { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData( new User { Key = 1, DiscordId = 1001624962538426398, Balance = 1000000 });
        
        modelBuilder.Entity<User>().Property(b => b.DiscordId).HasColumnType("bigint(10) unsigned");
        modelBuilder.Entity<Twink>().Property(a => a.DiscordId).HasColumnType("bigint(10) unsigned");
        modelBuilder.Entity<Shop>().Property(a => a.RoleId).HasColumnType("bigint(10) unsigned");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.LogTo(Console.WriteLine);
        
        const string connectionString = "server=localhost; user=root; password=4AnAl1PEnetRator7; database=polniykal;";
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}