# Model danych — encje i relacje

## Encje domenowe

### League (Liga)

```csharp
public class League
{
    public Guid Id { get; set; }
    public string Name { get; set; }           // max 100 znaków, wymagane
    public string? Description { get; set; }   // max 500 znaków, opcjonalne
    public DateOnly StartDate { get; set; }    // wymagane
    public DateOnly? EndDate { get; set; }     // opcjonalne
    public DateTime CreatedAt { get; set; }    // ustawiane automatycznie
    public bool IsActive { get; set; }         // domyślnie true

    // Relacje
    public ICollection<Race> Races { get; set; }
    public ICollection<LeagueAdmin> LeagueAdmins { get; set; }
}
```

### Race (Wyścig)

```csharp
public class Race
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }           // FK → League
    public string Name { get; set; }             // max 100 znaków, wymagane
    public string? Location { get; set; }        // max 200 znaków, opcjonalne
    public DateTime RaceDate { get; set; }       // data i godzina wyścigu
    public int NumberOfLaps { get; set; }        // liczba okrążeń, min 1
    public string? Notes { get; set; }           // max 1000 znaków, opcjonalne
    public DateTime CreatedAt { get; set; }

    // Relacje
    public League League { get; set; }
    public ICollection<RaceResult> Results { get; set; }
}
```

### Driver (Zawodnik)

```csharp
public class Driver
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }        // max 50 znaków, wymagane
    public string LastName { get; set; }         // max 50 znaków, wymagane
    public string? RacingNumber { get; set; }    // max 10 znaków, unikalny w ramach ligi (opcjonalne)
    public DateOnly? DateOfBirth { get; set; }   // opcjonalne
    public DateTime CreatedAt { get; set; }

    // Relacje
    public ICollection<RaceResult> Results { get; set; }
    public User? User { get; set; }              // opcjonalne powiązanie z kontem
}
```

### Kart (Gokart)

```csharp
public class Kart
{
    public Guid Id { get; set; }
    public string Number { get; set; }           // max 10 znaków, wymagane, unikalny
    public string? Model { get; set; }           // max 100 znaków, opcjonalne
    public string? Category { get; set; }        // max 50 znaków, np. "Senior", "Junior"
    public bool IsActive { get; set; }           // domyślnie true

    // Relacje
    public ICollection<RaceResult> Results { get; set; }
}
```

### RaceResult (Wynik wyścigu)

```csharp
public class RaceResult
{
    public Guid Id { get; set; }
    public Guid RaceId { get; set; }             // FK → Race
    public Guid DriverId { get; set; }           // FK → Driver
    public Guid KartId { get; set; }             // FK → Kart
    public long LapTimeMs { get; set; }          // czas najszybszego okrążenia w ms
    public long TotalTimeMs { get; set; }        // łączny czas przejazdu w ms
    public int StartingPosition { get; set; }    // pozycja startowa, min 1
    public int? FinishingPosition { get; set; }  // null = DNF
    public int LapsCompleted { get; set; }       // ukończone okrążenia
    public ResultStatus Status { get; set; }     // enum (patrz niżej)
    public string? Notes { get; set; }           // max 200 znaków
    public DateTime CreatedAt { get; set; }

    // Relacje
    public Race Race { get; set; }
    public Driver Driver { get; set; }
    public Kart Kart { get; set; }
}

public enum ResultStatus
{
    Finished = 0,       // ukończył wyścig
    DNF = 1,            // Did Not Finish — nie ukończył
    DNS = 2,            // Did Not Start — nie wystartował
    Disqualified = 3    // zdyskwalifikowany
}
```

### User (Użytkownik)

```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }            // max 256 znaków, unikalny, wymagane
    public string PasswordHash { get; set; }     // BCrypt hash, nigdy nie zwracać w API
    public string? FirstName { get; set; }       // max 50 znaków
    public string? LastName { get; set; }        // max 50 znaków
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }           // domyślnie true

    // Relacje
    public ICollection<UserRole> UserRoles { get; set; }
    public Driver? Driver { get; set; }          // opcjonalne powiązanie z zawodnikiem
    public Guid? DriverId { get; set; }
}
```

### Role (Rola)

```csharp
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }             // "Admin" lub "User"

    // Relacje
    public ICollection<UserRole> UserRoles { get; set; }
}
```

### UserRole (tabela pośrednia User ↔ Role)

```csharp
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
```

### LeagueAdmin (tabela pośrednia League ↔ User — administratorzy ligi)

```csharp
public class LeagueAdmin
{
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; }

    public League League { get; set; }
    public User User { get; set; }
}
```

---

## Relacje

| Od | Do | Typ | Opis |
|---|---|---|---|
| League | Race | 1:N | Liga zawiera wiele wyścigów |
| Race | RaceResult | 1:N | Wyścig ma wiele wyników |
| Driver | RaceResult | 1:N | Zawodnik ma wiele wyników |
| Kart | RaceResult | 1:N | Gokart pojawia się w wielu wynikach |
| User | Driver | 1:0..1 | Użytkownik może być powiązany z zawodnikiem |
| User | UserRole | 1:N | Użytkownik ma wiele ról |
| Role | UserRole | 1:N | Rola przypisana do wielu użytkowników |
| League | LeagueAdmin | 1:N | Liga ma wielu administratorów |
| User | LeagueAdmin | 1:N | Użytkownik może administrować wieloma ligami |

---

## Konfiguracja EF Core (ApplicationDbContext)

Plik: `Data/ApplicationDbContext.cs`

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<League> Leagues { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Kart> Karts { get; set; }
    public DbSet<RaceResult> RaceResults { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<LeagueAdmin> LeagueAdmins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserRole — klucz złożony
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // LeagueAdmin — klucz złożony
        modelBuilder.Entity<LeagueAdmin>()
            .HasKey(la => new { la.LeagueId, la.UserId });

        // User.Email — unikalny
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        // Kart.Number — unikalny
        modelBuilder.Entity<Kart>()
            .HasIndex(k => k.Number).IsUnique();

        // RaceResult — unikalny zawodnik w jednym wyścigu
        modelBuilder.Entity<RaceResult>()
            .HasIndex(r => new { r.RaceId, r.DriverId }).IsUnique();

        // Seed ról
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId  = Guid.Parse("22222222-2222-2222-2222-222222222222");
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = userRoleId,  Name = "User"  }
        );
    }
}
```

---

## Konwencja nazw tabel w PostgreSQL

EF Core domyślnie używa nazwy DbSet jako nazwy tabeli. Dodaj do `OnModelCreating` albo użyj atrybutu `[Table("races")]`:

| Encja | Tabela |
|---|---|
| League | leagues |
| Race | races |
| Driver | drivers |
| Kart | karts |
| RaceResult | race_results |
| User | users |
| Role | roles |
| UserRole | user_roles |
| LeagueAdmin | league_admins |

Skonfiguruj snake_case przez paczke `EFCore.NamingConventions`:

```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());
```

---

## Czas — format przechowywania

- `LapTimeMs` i `TotalTimeMs` przechowuj jako `bigint` (milisekundy)
- Formatowanie czasu na frontend: `mm:ss.fff`
- Przykład: 94532 ms → `01:34.532`
- Funkcja formatująca (TypeScript):

```typescript
formatTime(ms: number): string {
  const minutes = Math.floor(ms / 60000);
  const seconds = Math.floor((ms % 60000) / 1000);
  const millis  = ms % 1000;
  return `${String(minutes).padStart(2,'0')}:${String(seconds).padStart(2,'0')}.${String(millis).padStart(3,'0')}`;
}
```
