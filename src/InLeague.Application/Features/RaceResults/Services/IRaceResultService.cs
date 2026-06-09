using InLeague.Application.Features.RaceResults.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Application.Features.RaceResults.Services;

public interface IRaceResultService
{
    Task<IEnumerable<RaceResultDto>> GetByRaceIdAsync(Guid raceId);
    Task<RaceResultDto?> GetByIdAsync(Guid raceId, Guid id);
    Task<RaceResultDto?> CreateAsync(Guid raceId, CreateRaceResultDto dto);
    Task<RaceResultDto?> UpdateAsync(Guid raceId, Guid id, UpdateRaceResultDto dto);
    Task<bool> DeleteAsync(Guid raceId, Guid id);
}
