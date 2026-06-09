using Microsoft.EntityFrameworkCore;

namespace InLeague.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<League> Leagues { get; set; } = null!;
    public DbSet<Race> Races { get; set; } = null!;
    public DbSet<Driver> Drivers { get; set; } = null!;
    public DbSet<Kart> Karts { get; set; } = null!;
    public DbSet<RaceResult> RaceResults { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<LeagueAdmin> LeagueAdmins { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserRole — klucz zlozony
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // LeagueAdmin — klucz zlozony
        modelBuilder.Entity<LeagueAdmin>()
            .HasKey(la => new { la.LeagueId, la.UserId });

        // User.Email — unikalny
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        // Kart.Number — unikalny
        modelBuilder.Entity<Kart>()
            .HasIndex(k => k.Number).IsUnique();

        // RaceResult — unikalny zawodnik w jednym wyscigu
        modelBuilder.Entity<RaceResult>()
            .HasIndex(r => new { r.RaceId, r.DriverId }).IsUnique();

        // Seed rol
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId  = Guid.Parse("22222222-2222-2222-2222-222222222222");
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = userRoleId,  Name = "User"  }
        );
    }
}
