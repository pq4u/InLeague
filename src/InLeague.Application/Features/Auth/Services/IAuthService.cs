using InLeague.Application.Features.Auth.DTOs;
using System;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Auth.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<UserInfoDto?> GetCurrentUserAsync(Guid userId);
}
