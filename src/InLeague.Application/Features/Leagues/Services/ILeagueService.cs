namespace InLeague.Application.Features.Leagues.Services;

public interface ILeagueService
{
    Task<IEnumerable<LeagueDto>> GetAllAsync(bool? isActive = null);
    Task<LeagueDto?> GetByIdAsync(Guid id);
    Task<LeagueDto> CreateAsync(CreateLeagueDto dto);
    Task<LeagueDto?> UpdateAsync(Guid id, UpdateLeagueDto dto);
    Task<bool> DeleteAsync(Guid id);
}
