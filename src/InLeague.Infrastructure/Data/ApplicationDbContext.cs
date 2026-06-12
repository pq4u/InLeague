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
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<LeagueAdmin>()
            .HasKey(la => new { la.LeagueId, la.UserId });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Kart>()
            .HasIndex(k => k.Number).IsUnique();

        modelBuilder.Entity<RaceResult>()
            .HasIndex(r => new { r.RaceId, r.DriverId }).IsUnique();

        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId  = Guid.Parse("22222222-2222-2222-2222-222222222222");
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = userRoleId,  Name = "User"  }
        );
    }
}
