# Specyfikacja backendu — ASP.NET Core 8

## Inicjalizacja projektu

```bash
dotnet new webapi -n KartingLeague.Api --framework net8.0
cd KartingLeague.Api
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package EFCore.NamingConventions
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.AspNetCore
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.OpenApi
```

---

## Struktura katalogów projektu

```
KartingLeague.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── LeaguesController.cs
│   ├── RacesController.cs
│   ├── DriversController.cs
│   ├── KartsController.cs
│   └── RaceResultsController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── League.cs
│   ├── Race.cs
│   ├── Driver.cs
│   ├── Kart.cs
│   ├── RaceResult.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── UserRole.cs
│   ├── LeagueAdmin.cs
│   └── Enums/
│       └── ResultStatus.cs
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequestDto.cs
│   │   ├── RegisterRequestDto.cs
│   │   └── AuthResponseDto.cs
│   ├── League/
│   │   ├── LeagueDto.cs
│   │   ├── CreateLeagueDto.cs
│   │   └── UpdateLeagueDto.cs
│   ├── Race/
│   │   ├── RaceDto.cs
│   │   ├── CreateRaceDto.cs
│   │   └── UpdateRaceDto.cs
│   ├── Driver/
│   │   ├── DriverDto.cs
│   │   ├── CreateDriverDto.cs
│   │   └── UpdateDriverDto.cs
│   ├── Kart/
│   │   ├── KartDto.cs
│   │   ├── CreateKartDto.cs
│   │   └── UpdateKartDto.cs
│   └── RaceResult/
│       ├── RaceResultDto.cs
│       ├── CreateRaceResultDto.cs
│       └── UpdateRaceResultDto.cs
├── Repositories/
│   ├── Interfaces/
│   │   ├── ILeagueRepository.cs
│   │   ├── IRaceRepository.cs
│   │   ├── IDriverRepository.cs
│   │   ├── IKartRepository.cs
│   │   ├── IRaceResultRepository.cs
│   │   └── IUserRepository.cs
│   ├── LeagueRepository.cs
│   ├── RaceRepository.cs
│   ├── DriverRepository.cs
│   ├── KartRepository.cs
│   ├── RaceResultRepository.cs
│   └── UserRepository.cs
├── Services/
│   ├── Interfaces/
│   │   ├── ILeagueService.cs
│   │   ├── IRaceService.cs
│   │   ├── IDriverService.cs
│   │   ├── IKartService.cs
│   │   ├── IRaceResultService.cs
│   │   └── IAuthService.cs
│   ├── LeagueService.cs
│   ├── RaceService.cs
│   ├── DriverService.cs
│   ├── KartService.cs
│   ├── RaceResultService.cs
│   └── AuthService.cs
├── Validators/
│   ├── CreateLeagueDtoValidator.cs
│   ├── CreateRaceDtoValidator.cs
│   ├── CreateDriverDtoValidator.cs
│   ├── CreateKartDtoValidator.cs
│   ├── CreateRaceResultDtoValidator.cs
│   └── LoginRequestDtoValidator.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Mappings/
│   └── MappingProfile.cs
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

---

## Program.cs — pełna konfiguracja

```csharp
using FluentValidation;
using KartingLeague.Api.Data;
using KartingLeague.Api.Middleware;
using KartingLeague.Api.Repositories;
using KartingLeague.Api.Repositories.Interfaces;
using KartingLeague.Api.Services;
using KartingLeague.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- DbContext ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// --- AutoMapper ---
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// --- FluentValidation ---
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --- Repozytoria ---
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IKartRepository, KartRepository>();
builder.Services.AddScoped<IRaceResultRepository, RaceResultRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// --- Serwisy ---
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IRaceService, RaceService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IKartService, KartService>();
builder.Services.AddScoped<IRaceResultService, RaceResultService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- JWT ---
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Karting League API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Wpisz: Bearer {token}",
        Name        = "Authorization",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Middleware pipeline ---
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- Auto-migracje przy starcie (dev only) ---
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
```

---

## appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=karting_league;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "ZMIEN_NA_DLUGI_LOSOWY_KLUCZ_MIN_32_ZNAKI_!!",
    "Issuer": "KartingLeagueApi",
    "Audience": "KartingLeagueFrontend",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## Wzorzec Repozytorium

### Interfejs bazowy (opcjonalny)

```csharp
// Repositories/Interfaces/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### Przykład: ILeagueRepository

```csharp
public interface ILeagueRepository : IRepository<League>
{
    Task<IEnumerable<League>> GetAllWithRacesAsync();
    Task<League?> GetByIdWithRacesAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
```

### Przykład: LeagueRepository

```csharp
public class LeagueRepository : ILeagueRepository
{
    private readonly ApplicationDbContext _context;

    public LeagueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<League?> GetByIdAsync(Guid id)
        => await _context.Leagues.FindAsync(id);

    public async Task<League?> GetByIdWithRacesAsync(Guid id)
        => await _context.Leagues
            .Include(l => l.Races)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<League>> GetAllAsync()
        => await _context.Leagues.ToListAsync();

    public async Task<IEnumerable<League>> GetAllWithRacesAsync()
        => await _context.Leagues
            .Include(l => l.Races)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<bool> ExistsAsync(Guid id)
        => await _context.Leagues.AnyAsync(l => l.Id == id);

    public async Task<League> CreateAsync(League entity)
    {
        entity.Id        = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.Leagues.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<League> UpdateAsync(League entity)
    {
        _context.Leagues.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(League entity)
    {
        _context.Leagues.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

---

## Wzorzec Serwisu

### Przykład: ILeagueService

```csharp
public interface ILeagueService
{
    Task<IEnumerable<LeagueDto>> GetAllAsync();
    Task<LeagueDto?> GetByIdAsync(Guid id);
    Task<LeagueDto> CreateAsync(CreateLeagueDto dto);
    Task<LeagueDto?> UpdateAsync(Guid id, UpdateLeagueDto dto);
    Task<bool> DeleteAsync(Guid id);
}
```

### Przykład: LeagueService

```csharp
public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _repository;
    private readonly IMapper _mapper;

    public LeagueService(ILeagueRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper     = mapper;
    }

    public async Task<IEnumerable<LeagueDto>> GetAllAsync()
    {
        var leagues = await _repository.GetAllWithRacesAsync();
        return _mapper.Map<IEnumerable<LeagueDto>>(leagues);
    }

    public async Task<LeagueDto?> GetByIdAsync(Guid id)
    {
        var league = await _repository.GetByIdWithRacesAsync(id);
        return league is null ? null : _mapper.Map<LeagueDto>(league);
    }

    public async Task<LeagueDto> CreateAsync(CreateLeagueDto dto)
    {
        var league = _mapper.Map<League>(dto);
        var created = await _repository.CreateAsync(league);
        return _mapper.Map<LeagueDto>(created);
    }

    public async Task<LeagueDto?> UpdateAsync(Guid id, UpdateLeagueDto dto)
    {
        var league = await _repository.GetByIdAsync(id);
        if (league is null) return null;

        _mapper.Map(dto, league);
        var updated = await _repository.UpdateAsync(league);
        return _mapper.Map<LeagueDto>(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var league = await _repository.GetByIdAsync(id);
        if (league is null) return false;

        await _repository.DeleteAsync(league);
        return true;
    }
}
```

---

## ExceptionHandlingMiddleware

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nieobsłużony wyjątek");
            context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error   = "Wystąpił błąd serwera",
                message = ex.Message
            });
        }
    }
}
```

---

## MappingProfile (AutoMapper)

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // League
        CreateMap<League, LeagueDto>()
            .ForMember(d => d.RaceCount, o => o.MapFrom(s => s.Races.Count));
        CreateMap<CreateLeagueDto, League>();
        CreateMap<UpdateLeagueDto, League>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        // Race
        CreateMap<Race, RaceDto>();
        CreateMap<CreateRaceDto, Race>();
        CreateMap<UpdateRaceDto, Race>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        // Driver
        CreateMap<Driver, DriverDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
        CreateMap<CreateDriverDto, Driver>();
        CreateMap<UpdateDriverDto, Driver>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        // Kart
        CreateMap<Kart, KartDto>();
        CreateMap<CreateKartDto, Kart>();
        CreateMap<UpdateKartDto, Kart>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        // RaceResult
        CreateMap<RaceResult, RaceResultDto>()
            .ForMember(d => d.DriverName, o => o.MapFrom(s => $"{s.Driver.FirstName} {s.Driver.LastName}"))
            .ForMember(d => d.KartNumber,  o => o.MapFrom(s => s.Kart.Number));
        CreateMap<CreateRaceResultDto, RaceResult>();
        CreateMap<UpdateRaceResultDto, RaceResult>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```
