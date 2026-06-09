using InLeague.Application.Features.Races.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Races.Services;

public interface IRaceService
{
    Task<IEnumerable<RaceDto>> GetByLeagueIdAsync(Guid leagueId);
    Task<RaceDto?> GetByIdAsync(Guid id);
    Task<RaceDto?> CreateAsync(Guid leagueId, CreateRaceDto dto);
    Task<RaceDto?> UpdateAsync(Guid id, UpdateRaceDto dto);
    Task<bool> DeleteAsync(Guid id);
}
