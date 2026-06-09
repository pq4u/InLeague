# Lista zadań implementacyjnych — kolejność dla agenta AI

Każde zadanie jest atomowe: ma jasny cel, pliki do utworzenia i kryterium ukończenia.
Realizuj zadania **w kolejności** — późniejsze zadania zależą od wcześniejszych.

---

## FAZA 1 — Inicjalizacja projektu

### TASK-001: Inicjalizacja solucji .NET
**Cel:** Utworzyć strukturę projektu backendowego.

```bash
mkdir karting-league && cd karting-league
mkdir -p backend frontend

cd backend
dotnet new sln -n KartingLeague
dotnet new webapi -n KartingLeague.Api --framework net8.0 --no-openapi false
dotnet sln add KartingLeague.Api/KartingLeague.Api.csproj

cd KartingLeague.Api
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.*
dotnet add package EFCore.NamingConventions
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.AspNetCore
dotnet add package Swashbuckle.AspNetCore
```

**Kryterium:** `dotnet build` kończy się bez błędów.

---

### TASK-002: Inicjalizacja projektu Angular
**Cel:** Utworzyć projekt frontendowy.

```bash
cd ../../frontend
ng new karting-league-app --standalone --routing --style=scss --skip-git
cd karting-league-app
ng add @angular/material --skip-confirmation
npm install jwt-decode
```

**Kryterium:** `ng serve` uruchamia się bez błędów.

---

## FAZA 2 — Backend: modele i baza danych

### TASK-003: Utworzenie modeli domenowych
**Pliki do utworzenia** (wg `docs/01-data-model.md`):
- `Models/League.cs`
- `Models/Race.cs`
- `Models/Driver.cs`
- `Models/Kart.cs`
- `Models/RaceResult.cs`
- `Models/User.cs`
- `Models/Role.cs`
- `Models/UserRole.cs`
- `Models/LeagueAdmin.cs`
- `Models/Enums/ResultStatus.cs`

**Kryterium:** `dotnet build` bez błędów.

---

### TASK-004: ApplicationDbContext i konfiguracja EF Core
**Pliki do utworzenia:**
- `Data/ApplicationDbContext.cs` — wg `docs/01-data-model.md` sekcja "Konfiguracja EF Core"

**Konfiguracja w `Program.cs`:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());
```

**Plik `appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=karting_league;Username=postgres;Password=postgres"
  }
}
```

**Kryterium:** `dotnet build` bez błędów.

---

### TASK-005: Pierwsza migracja EF Core
```bash
cd backend/KartingLeague.Api
dotnet tool install --global dotnet-ef  # jeśli niezainstalowane
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Kryterium:** Baza PostgreSQL zawiera wszystkie tabele. Sprawdź przez `psql` lub pgAdmin.

---

## FAZA 3 — Backend: warstwa dostępu do danych

### TASK-006: Interfejsy repozytoriów
**Pliki do utworzenia** (wg `docs/02-backend-spec.md`):
- `Repositories/Interfaces/ILeagueRepository.cs`
- `Repositories/Interfaces/IRaceRepository.cs`
- `Repositories/Interfaces/IDriverRepository.cs`
- `Repositories/Interfaces/IKartRepository.cs`
- `Repositories/Interfaces/IRaceResultRepository.cs`
- `Repositories/Interfaces/IUserRepository.cs`

Każdy interfejs dziedziczy po `IRepository<T>` i dodaje metody specyficzne dla encji (np. `GetByIdWithRacesAsync`, `GetByEmailAsync`).

**Kryterium:** Kompilacja bez błędów.

---

### TASK-007: Implementacje repozytoriów
**Pliki do utworzenia:**
- `Repositories/LeagueRepository.cs`
- `Repositories/RaceRepository.cs`
- `Repositories/DriverRepository.cs`
- `Repositories/KartRepository.cs`
- `Repositories/RaceResultRepository.cs`
- `Repositories/UserRepository.cs`

`UserRepository` musi implementować `GetByEmailAsync(string email)` i `GetRoleByNameAsync(string name)`.

**Kryterium:** Kompilacja bez błędów.

---

## FAZA 4 — Backend: DTO i mapowania

### TASK-008: Definicje DTO
**Pliki do utworzenia** (wg `docs/02-backend-spec.md` i `docs/03-api-endpoints.md`):

```
DTOs/
├── Auth/LoginRequestDto.cs
├── Auth/RegisterRequestDto.cs
├── Auth/AuthResponseDto.cs
├── Auth/UserInfoDto.cs
├── League/LeagueDto.cs
├── League/CreateLeagueDto.cs
├── League/UpdateLeagueDto.cs
├── Race/RaceDto.cs
├── Race/CreateRaceDto.cs
├── Race/UpdateRaceDto.cs
├── Driver/DriverDto.cs
├── Driver/CreateDriverDto.cs
├── Driver/UpdateDriverDto.cs
├── Kart/KartDto.cs
├── Kart/CreateKartDto.cs
├── Kart/UpdateKartDto.cs
├── RaceResult/RaceResultDto.cs
├── RaceResult/CreateRaceResultDto.cs
└── RaceResult/UpdateRaceResultDto.cs
```

Pola DTO muszą odpowiadać przykładowym JSON z `docs/03-api-endpoints.md`.

**Kryterium:** Kompilacja bez błędów.

---

### TASK-009: MappingProfile (AutoMapper)
**Plik:** `Mappings/MappingProfile.cs` — wg `docs/02-backend-spec.md` sekcja "MappingProfile".

Zarejestruj w `Program.cs`: `builder.Services.AddAutoMapper(typeof(Program).Assembly);`

**Kryterium:** Kompilacja bez błędów.

---

### TASK-010: Walidatory FluentValidation
**Pliki do utworzenia:**
- `Validators/LoginRequestDtoValidator.cs`
- `Validators/RegisterRequestDtoValidator.cs`
- `Validators/CreateLeagueDtoValidator.cs`
- `Validators/CreateRaceDtoValidator.cs`
- `Validators/CreateDriverDtoValidator.cs`
- `Validators/CreateKartDtoValidator.cs`
- `Validators/CreateRaceResultDtoValidator.cs`

Reguły walidacji wg sekcji "Walidacja" przy każdym endpoincie w `docs/03-api-endpoints.md`.

Zarejestruj w `Program.cs`: `builder.Services.AddValidatorsFromAssemblyContaining<Program>();`

**Kryterium:** Kompilacja bez błędów.

---

## FAZA 5 — Backend: serwisy i autentykacja

### TASK-011: AuthService i JWT
**Pliki do utworzenia:**
- `Services/Interfaces/IAuthService.cs`
- `Services/AuthService.cs` — wg `docs/04-auth-spec.md`

Konfiguracja JWT w `Program.cs` — wg `docs/02-backend-spec.md` sekcja "Program.cs".

Dodaj do `appsettings.json`:
```json
"Jwt": {
  "Key": "ZMIEN_NA_KLUCZ_MINIMUM_32_ZNAKI_LOSOWY!!",
  "Issuer": "KartingLeagueApi",
  "Audience": "KartingLeagueFrontend",
  "ExpirationMinutes": 60
}
```

**Kryterium:** Kompilacja bez błędów.

---

### TASK-012: Serwisy domenowe
**Pliki do utworzenia:**
- `Services/Interfaces/ILeagueService.cs` + `Services/LeagueService.cs`
- `Services/Interfaces/IRaceService.cs` + `Services/RaceService.cs`
- `Services/Interfaces/IDriverService.cs` + `Services/DriverService.cs`
- `Services/Interfaces/IKartService.cs` + `Services/KartService.cs`
- `Services/Interfaces/IRaceResultService.cs` + `Services/RaceResultService.cs`

Wzorzec implementacji wg `docs/02-backend-spec.md` sekcja "Wzorzec Serwisu".

**Kryterium:** Kompilacja bez błędów.

---

### TASK-013: ExceptionHandlingMiddleware
**Plik:** `Middleware/ExceptionHandlingMiddleware.cs` — wg `docs/02-backend-spec.md`.

Zarejestruj jako pierwszy middleware w `Program.cs`:
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

**Kryterium:** Kompilacja bez błędów.

---

## FAZA 6 — Backend: kontrolery

### TASK-014: AuthController
**Plik:** `Controllers/AuthController.cs` — wg `docs/04-auth-spec.md` sekcja "AuthController".

Endpointy: `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me`

**Test manualny:**
1. Zarejestruj użytkownika → powinien zwrócić 201 z tokenem
2. Zaloguj się → powinien zwrócić 200 z tokenem
3. GET /api/auth/me z tokenem → powinien zwrócić dane użytkownika
4. GET /api/auth/me bez tokenu → powinien zwrócić 401

---

### TASK-015: LeaguesController
**Plik:** `Controllers/LeaguesController.cs` — wg `docs/03-api-endpoints.md` sekcja "Ligi".

Przykład implementacji wg `docs/03-api-endpoints.md` sekcja "Przykład kontrolera".

**Test manualny przez Swagger:**
1. GET /api/leagues — publiczne, 200
2. POST /api/leagues bez tokenu — 401
3. POST /api/leagues z tokenem user (nie admin) — 403
4. POST /api/leagues z tokenem admina — 201

---

### TASK-016: RacesController
**Plik:** `Controllers/RacesController.cs`

Route: `[Route("api/leagues/{leagueId:guid}/races")]`

Wszystkie metody walidują, że `leagueId` istnieje w bazie → 404 jeśli nie.

**Endpointy wg `docs/03-api-endpoints.md` sekcja "Wyścigi".**

---

### TASK-017: DriversController i KartsController
**Pliki:**
- `Controllers/DriversController.cs` — route `api/drivers`
- `Controllers/KartsController.cs` — route `api/karts`

**Endpointy wg `docs/03-api-endpoints.md`.**

---

### TASK-018: RaceResultsController
**Plik:** `Controllers/RaceResultsController.cs`

Route: `[Route("api/races/{raceId:guid}/results")]`

Serwis musi sprawdzić:
- Czy `raceId` istnieje → 404
- Czy `driverId` istnieje → 400
- Czy `kartId` istnieje → 400
- Czy wynik dla tego zawodnika w tym wyścigu już istnieje → 409

---

### TASK-019: Rejestracja DI w Program.cs
Upewnij się, że wszystkie serwisy i repozytoria są zarejestrowane:

```csharp
// Repozytoria
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IKartRepository, KartRepository>();
builder.Services.AddScoped<IRaceResultRepository, RaceResultRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Serwisy
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<IRaceService, RaceService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IKartService, KartService>();
builder.Services.AddScoped<IRaceResultService, RaceResultService>();
builder.Services.AddScoped<IAuthService, AuthService>();
```

**Kryterium:** `dotnet run` uruchamia API, Swagger dostępny pod `/swagger`.

---

## FAZA 7 — Frontend: rdzeń aplikacji

### TASK-020: Modele TypeScript
**Pliki do utworzenia** wg `docs/05-frontend-spec.md` sekcja "Modele":
- `src/app/core/models/auth.model.ts`
- `src/app/core/models/league.model.ts`
- `src/app/core/models/race.model.ts`
- `src/app/core/models/driver.model.ts`
- `src/app/core/models/kart.model.ts`
- `src/app/core/models/race-result.model.ts`

---

### TASK-021: Environment i konfiguracja
**Pliki:**
- `src/environments/environment.ts`
- `src/environments/environment.production.ts`

Wg `docs/05-frontend-spec.md` sekcja "environment.ts".

---

### TASK-022: AuthService + JWT Interceptor + Guards
**Pliki do utworzenia** wg `docs/05-frontend-spec.md`:
- `src/app/core/services/auth.service.ts`
- `src/app/core/interceptors/jwt.interceptor.ts`
- `src/app/core/interceptors/error.interceptor.ts`
- `src/app/core/guards/auth.guard.ts`
- `src/app/core/guards/role.guard.ts`

Zaktualizuj `src/app/app.config.ts` wg specyfikacji.

---

### TASK-023: Serwisy API
**Pliki do utworzenia** (wzorzec wg `docs/05-frontend-spec.md` sekcja "Przykładowy serwis"):
- `src/app/core/services/league.service.ts`
- `src/app/core/services/race.service.ts`
- `src/app/core/services/driver.service.ts`
- `src/app/core/services/kart.service.ts`
- `src/app/core/services/race-result.service.ts`

---

### TASK-024: Routing
**Plik:** `src/app/app.routes.ts` — wg `docs/05-frontend-spec.md` sekcja "Routing".

---

### TASK-025: Shared — pipe i navbar
**Pliki do utworzenia:**
- `src/app/shared/pipes/race-time.pipe.ts` — wg `docs/05-frontend-spec.md`
- `src/app/shared/components/navbar/navbar.component.ts` + `.html`

Navbar wyświetla: logo, link do listy lig, przycisk "Zaloguj" jeśli niezalogowany, email + "Wyloguj" jeśli zalogowany, link "Panel admina" jeśli rola Admin.

---

## FAZA 8 — Frontend: komponenty

### TASK-026: Komponenty auth (Login, Register)
**Pliki:**
- `src/app/features/auth/login/login.component.ts` + `.html`
- `src/app/features/auth/register/register.component.ts` + `.html`

Używają reaktywnych formularzy Angular (`FormBuilder`, `Validators`). Po sukcesie przekieruj na `/leagues`.

---

### TASK-027: Komponenty lig (LeagueList, LeagueDetail, LeagueForm)
**Pliki:**
- `src/app/features/leagues/league-list/` — tabela lig, przyciski "Szczegóły" i (admin) "Edytuj"/"Usuń"
- `src/app/features/leagues/league-detail/` — info o lidze + lista wyścigów
- `src/app/features/leagues/league-form/` — formularz tworzenia/edycji (ten sam komponent dla obu)

---

### TASK-028: Komponenty wyścigów (RaceDetail, RaceForm)
**Pliki:**
- `src/app/features/races/race-detail/` — info o wyścigu + tabela wyników posortowana po pozycji
- `src/app/features/races/race-form/` — formularz tworzenia/edycji wyścigu

---

### TASK-029: Formularz wyników (ResultForm)
**Plik:** `src/app/features/results/result-form/`

Formularz zawiera:
- Dropdown zawodników (ładuje GET /api/drivers)
- Dropdown gokartów (ładuje GET /api/karts?isActive=true)
- Pola: pozycja startowa, pozycja końcowa, czas okrążenia (mm:ss.fff), łączny czas, liczba okrążeń, status
- Konwersja czasu mm:ss.fff → ms przed wysłaniem

---

### TASK-030: Komponenty zawodników i gokartów
**Pliki:**
- `src/app/features/drivers/driver-list/` + `driver-form/`
- `src/app/features/karts/kart-list/` + `kart-form/`

---

## FAZA 9 — Docker i wdrożenie

### TASK-031: Dockerfile backendu
**Plik:** `backend/KartingLeague.Api/Dockerfile` — wg `docs/06-docker-spec.md`.

**Test:** `docker build -t karting-api ./backend/KartingLeague.Api`

---

### TASK-032: Dockerfile i nginx frontendu
**Pliki:**
- `frontend/karting-league-app/Dockerfile`
- `frontend/karting-league-app/nginx.conf`

Wg `docs/06-docker-spec.md`.

**Test:** `docker build -t karting-frontend ./frontend/karting-league-app`

---

### TASK-033: Docker Compose
**Pliki:**
- `docker-compose.yml`
- `.env` (nie commituj)

Wg `docs/06-docker-spec.md`.

**Test:** `docker compose up --build -d` — wszystkie 3 serwisy działają.

---

## FAZA 10 — Testy

### TASK-034: Testy jednostkowe serwisów
```bash
cd backend
dotnet new xunit -n KartingLeague.Tests
dotnet sln add KartingLeague.Tests/KartingLeague.Tests.csproj
cd KartingLeague.Tests
dotnet add package Moq
dotnet add package AutoMapper
dotnet add reference ../KartingLeague.Api/KartingLeague.Api.csproj
```

**Testy do napisania:**
- `LeagueServiceTests` — GetAll, GetById, Create, Update, Delete
- `AuthServiceTests` — Login (poprawne dane), Login (błędne dane), Register (nowy user), Register (duplikat)
- `RaceResultServiceTests` — Create (duplikat zawodnika w wyścigu → wyjątek)

---

### TASK-035: Seed danych testowych
**Plik:** `Data/DataSeeder.cs`

```csharp
public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Leagues.AnyAsync()) return;

        // Utwórz ligę przykładową
        var league = new League { /* ... */ };
        // Utwórz wyścigi, zawodników, gokarty
        // Utwórz konto admina: admin@karting.pl / Admin123!
        // Utwórz konto usera: user@karting.pl / User123!
    }
}
```

Wywołaj w `Program.cs` podczas `Development`:
```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DataSeeder.SeedAsync(context);
}
```

---

## Checklist ukończenia projektu

- [ ] TASK-001 do TASK-005: Backend skeleton + migracja
- [ ] TASK-006 do TASK-010: Repozytoria, DTO, mapowania, walidatory
- [ ] TASK-011 do TASK-013: Serwisy, JWT, middleware
- [ ] TASK-014 do TASK-019: Wszystkie kontrolery + DI
- [ ] TASK-020 do TASK-025: Frontend core (modele, serwisy, guard, routing)
- [ ] TASK-026 do TASK-030: Wszystkie komponenty
- [ ] TASK-031 do TASK-033: Docker + Docker Compose
- [ ] TASK-034 do TASK-035: Testy + seed
- [ ] Swagger dokumentuje wszystkie endpointy
- [ ] Logowanie i wylogowywanie działa end-to-end
- [ ] Admin może tworzyć/edytować/usuwać
- [ ] User (niezalogowany) widzi ligi i wyniki, nie widzi przycisków admina
- [ ] Czas wyścigu formatowany poprawnie (mm:ss.fff)
