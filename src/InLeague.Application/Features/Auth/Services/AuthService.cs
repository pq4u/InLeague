using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Auth.DTOs;
using InLeague.Domain.Features.Users.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(IUnitOfWork uow, IConfiguration config)
    {
        _uow = uow;
        _config = config;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _uow.Users.GetByEmailAsync(dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        var (token, expiresAt) = GenerateJwtToken(user);
        return BuildAuthResponse(user, token, expiresAt);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existing = await _uow.Users.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new UserAlreadyExistsException("Użytkownik z tym adresem email już istnieje");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var userRole = await _uow.Users.GetRoleByNameAsync("User");
        if (userRole == null)
            throw new InvalidOperationException("Rola 'User' nie istnieje w systemie");

        user.UserRoles = new List<UserRole>
        {
            new UserRole { UserId = user.Id, RoleId = userRole.Id, Role = userRole }
        };

        await _uow.Users.CreateAsync(user);
        await _uow.SaveChangesAsync();

        var (token, expiresAt) = GenerateJwtToken(user);
        return BuildAuthResponse(user, token, expiresAt);
    }

    public async Task<UserInfoDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        return user?.ToDto();
    }

    private (string Token, DateTime ExpiresAt) GenerateJwtToken(User user)
    {
        var keyStr = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationMinutes = double.Parse(_config["Jwt:ExpirationMinutes"] ?? "60");
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenStr, expires);
    }

    private AuthResponseDto BuildAuthResponse(User user, string token, DateTime expiresAt)
    {
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = user.ToDto()
        };
    }
}
