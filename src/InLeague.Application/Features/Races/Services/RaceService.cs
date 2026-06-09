using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Races.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Races.Services;

public class RaceService : IRaceService
{
    private readonly IUnitOfWork _uow;

    public RaceService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<RaceDto>> GetByLeagueIdAsync(Guid leagueId)
    {
        var races = await _uow.Races.GetByLeagueIdAsync(leagueId);
        return races.Select(r => r.ToDto());
    }

    public async Task<RaceDto?> GetByIdAsync(Guid id)
    {
        var race = await _uow.Races.GetByIdWithResultsAsync(id);
        return race?.ToDto();
    }

    public async Task<RaceDto?> CreateAsync(Guid leagueId, CreateRaceDto dto)
    {
        if (!await _uow.Leagues.ExistsAsync(leagueId)) return null;

        var race = dto.ToEntity(leagueId);
        await _uow.Races.CreateAsync(race);
        await _uow.SaveChangesAsync();
        return race.ToDto();
    }

    public async Task<RaceDto?> UpdateAsync(Guid id, UpdateRaceDto dto)
    {
        var race = await _uow.Races.GetByIdAsync(id);
        if (race is null) return null;

        dto.UpdateEntity(race);
        await _uow.Races.UpdateAsync(race);
        await _uow.SaveChangesAsync();
        return race.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var race = await _uow.Races.GetByIdAsync(id);
        if (race is null) return false;

        await _uow.Races.DeleteAsync(race);
        await _uow.SaveChangesAsync();
        return true;
    }
}
