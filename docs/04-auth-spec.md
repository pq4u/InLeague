# Specyfikacja autentykacji i autoryzacji — JWT

## Schemat działania

```
1. POST /api/auth/login  →  serwer weryfikuje hasło (BCrypt)
2. Serwer generuje JWT z claimami (userId, email, roles)
3. Frontend przechowuje token w pamięci (lub localStorage)
4. Każdy request chroniony: Header "Authorization: Bearer <token>"
5. ASP.NET Core middleware waliduje token automatycznie
6. Kontroler odczytuje role z claims → [Authorize(Roles = "Admin")]
```

---

## Struktura tokenu JWT

### Header
```json
{ "alg": "HS256", "typ": "JWT" }
```

### Payload (claims)
```json
{
  "sub":   "uuid-użytkownika",
  "email": "jan@example.com",
  "role":  ["Admin"],
  "jti":   "uuid-tokenu",
  "iat":   1700000000,
  "exp":   1700003600
}
```

Claim `role` może być tablicą (dla wielu ról) lub stringiem (jedna rola). ASP.NET Core obsługuje oba warianty.

---

## AuthService — implementacja

```csharp
public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration  _config;
    private readonly IMapper          _mapper;

    public AuthService(IUserRepository userRepository, IConfiguration config, IMapper mapper)
    {
        _userRepository = userRepository;
        _config         = config;
        _mapper         = mapper;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        // Aktualizuj czas ostatniego logowania
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        return BuildAuthResponse(user, token);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("Użytkownik z tym adresem email już istnieje");

        var user = new User
        {
            Id           = Guid.NewGuid(),
            Email        = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName    = dto.FirstName,
            LastName     = dto.LastName,
            CreatedAt    = DateTime.UtcNow,
            IsActive     = true
        };

        // Przypisz domyślną rolę "User"
        var userRole = await _userRepository.GetRoleByNameAsync("User");
        user.UserRoles = new List<UserRole>
        {
            new UserRole { UserId = user.Id, RoleId = userRole!.Id }
        };

        await _userRepository.CreateAsync(user);

        var token = GenerateJwtToken(user);
        return BuildAuthResponse(user, token);
    }

    private string GenerateJwtToken(User user)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpirationMinutes"]!));

        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        // Dodaj każdą rolę jako osobny claim
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:            _config["Jwt:Issuer"],
            audience:          _config["Jwt:Audience"],
            claims:            claims,
            expires:           expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private AuthResponseDto BuildAuthResponse(User user, string token)
    {
        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new();
        return new AuthResponseDto
        {
            Token     = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpirationMinutes"]!)),
            User      = new UserInfoDto
            {
                Id        = user.Id,
                Email     = user.Email,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Roles     = roles,
                DriverId  = user.DriverId
            }
        };
    }
}
```

---

## DTO dla autentykacji

```csharp
public class LoginRequestDto
{
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
    public string  Email     { get; set; } = string.Empty;
    public string  Password  { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
}

public class AuthResponseDto
{
    public string      Token     { get; set; } = string.Empty;
    public DateTime    ExpiresAt { get; set; }
    public UserInfoDto User      { get; set; } = null!;
}

public class UserInfoDto
{
    public Guid        Id        { get; set; }
    public string      Email     { get; set; } = string.Empty;
    public string?     FirstName { get; set; }
    public string?     LastName  { get; set; }
    public List<string> Roles    { get; set; } = new();
    public Guid?       DriverId  { get; set; }
}
```

---

## Walidatory FluentValidation

```csharp
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane");
    }
}

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email")
            .MaximumLength(256).WithMessage("Email może mieć maksymalnie 256 znaków");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MinimumLength(8).WithMessage("Hasło musi mieć co najmniej 8 znaków")
            .Matches("[A-Z]").WithMessage("Hasło musi zawierać co najmniej jedną wielką literę")
            .Matches("[0-9]").WithMessage("Hasło musi zawierać co najmniej jedną cyfrę");

        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("Imię może mieć maksymalnie 50 znaków")
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Nazwisko może mieć maksymalnie 50 znaków")
            .When(x => x.LastName is not null);
    }
}
```

---

## AuthController

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Me), result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized(new { error = "Nieprawidłowy email lub hasło" });

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(userId);
        return user is null ? NotFound() : Ok(user);
    }
}
```

---

## Polityki autoryzacji — tabela

| Endpoint | Wymagane | Opis |
|---|---|---|
| `GET /api/leagues` | Publiczny | Wszyscy mogą przeglądać |
| `GET /api/leagues/{id}` | Publiczny | Wszyscy mogą przeglądać |
| `POST /api/leagues` | `[Authorize(Roles = "Admin")]` | Tylko admin |
| `PUT /api/leagues/{id}` | `[Authorize(Roles = "Admin")]` | Tylko admin |
| `DELETE /api/leagues/{id}` | `[Authorize(Roles = "Admin")]` | Tylko admin |
| `GET /api/leagues/{id}/races` | Publiczny | |
| `POST /api/leagues/{id}/races` | `[Authorize(Roles = "Admin")]` | |
| `GET /api/drivers` | Publiczny | |
| `POST /api/drivers` | `[Authorize(Roles = "Admin")]` | |
| `GET /api/karts` | Publiczny | |
| `POST /api/karts` | `[Authorize(Roles = "Admin")]` | |
| `GET /api/races/{id}/results` | Publiczny | |
| `POST /api/races/{id}/results` | `[Authorize(Roles = "Admin")]` | |
| `GET /api/auth/me` | `[Authorize]` | Zalogowany użytkownik |

---

## Odczytywanie userId z tokenu w kontrolerze

```csharp
// Metoda pomocnicza — dodaj do BaseController lub jako extension method
protected Guid GetCurrentUserId()
{
    var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    return Guid.Parse(claim!);
}

protected bool IsAdmin()
    => User.IsInRole("Admin");
```
