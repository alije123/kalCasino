using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace kalCasino.Database;

public sealed class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Shop> Shop { get; set; }
    public DbSet<LuckHour> LuckHour { get; set; }
    
    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Twink>().HasKey(u => new { u.User, u.DiscordId });
        modelBuilder.Entity<History>()
            .Property(h => h.ChangeDate)
            .HasDefaultValueSql("NOW()");
        // modelBuilder.Entity<User>().Property(b => b.DiscordId).HasColumnType("bigint(10)");
        // modelBuilder.Entity<Twink>().Property(a => a.DiscordId).HasColumnType("bigint(10)");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to mysql with connection string from app settings
        var connectionString = _configuration.GetConnectionString("WebApiDatabase");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}