using InLeague.Application.Features.Races.DTOs;
using InLeague.Application.Features.RaceResults.DTOs;
using InLeague.Application.Features.RaceResults.Mappings;
using InLeague.Domain.Features.Races;
using System;
using System.Linq;
using System.Collections.Generic;

namespace InLeague.Application.Features.Races.Mappings;

public static class RaceMappingExtensions
{
    public static RaceDto ToDto(this Race race)
    {
        return new RaceDto
        {
            Id = race.Id,
            LeagueId = race.LeagueId,
            Name = race.Name,
            Location = race.Location,
            RaceDate = race.RaceDate,
            NumberOfLaps = race.NumberOfLaps,
            Notes = race.Notes,
            CreatedAt = race.CreatedAt,
            ResultCount = race.Results?.Count ?? 0,
            Results = race.Results?.Select(r => r.ToDto()).ToList()
        };
    }

    public static Race ToEntity(this CreateRaceDto dto, Guid leagueId)
    {
        return new Race
        {
            LeagueId = leagueId,
            Name = dto.Name,
            Location = dto.Location,
            RaceDate = dto.RaceDate,
            NumberOfLaps = dto.NumberOfLaps,
            Notes = dto.Notes
        };
    }

    public static void UpdateEntity(this UpdateRaceDto dto, Race race)
    {
        if (dto.Name != null) race.Name = dto.Name;
        race.Location = dto.Location;
        if (dto.RaceDate.HasValue) race.RaceDate = dto.RaceDate.Value;
        if (dto.NumberOfLaps.HasValue) race.NumberOfLaps = dto.NumberOfLaps.Value;
        race.Notes = dto.Notes;
    }
}
