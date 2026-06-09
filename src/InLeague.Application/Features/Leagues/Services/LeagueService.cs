using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Leagues.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Leagues.Services;

public class LeagueService : ILeagueService
{
    private readonly IUnitOfWork _uow;

    public LeagueService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<LeagueDto>> GetAllAsync(bool? isActive = null)
    {
        var leagues = await _uow.Leagues.GetAllWithRacesAsync();
        if (isActive.HasValue)
            leagues = leagues.Where(l => l.IsActive == isActive.Value);

        return leagues.Select(l => l.ToDto());
    }

    public async Task<LeagueDto?> GetByIdAsync(Guid id)
    {
        var league = await _uow.Leagues.GetByIdWithRacesAsync(id);
        return league?.ToDto();
    }

    public async Task<LeagueDto> CreateAsync(CreateLeagueDto dto)
    {
        var league = dto.ToEntity();
        league.IsActive = true;
        await _uow.Leagues.CreateAsync(league);
        await _uow.SaveChangesAsync();
        return league.ToDto();
    }

    public async Task<LeagueDto?> UpdateAsync(Guid id, UpdateLeagueDto dto)
    {
        var league = await _uow.Leagues.GetByIdAsync(id);
        if (league is null) return null;

        dto.UpdateEntity(league);
        await _uow.Leagues.UpdateAsync(league);
        await _uow.SaveChangesAsync();
        return league.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var league = await _uow.Leagues.GetByIdWithRacesAsync(id);
        if (league is null || league.Races.Any()) return false;

        await _uow.Leagues.DeleteAsync(league);
        await _uow.SaveChangesAsync();
        return true;
    }
}
