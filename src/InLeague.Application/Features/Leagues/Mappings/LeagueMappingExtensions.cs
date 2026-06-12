namespace InLeague.Application.Features.Leagues.Mappings;

public static class LeagueMappingExtensions
{
    public static LeagueDto ToDto(this League league)
    {
        return new LeagueDto
        {
            Id = league.Id,
            Name = league.Name,
            Description = league.Description,
            StartDate = league.StartDate,
            EndDate = league.EndDate,
            IsActive = league.IsActive,
            CreatedAt = league.CreatedAt,
            RaceCount = league.Races?.Count ?? 0,
            Races = league.Races?.Select(r => r.ToDto()).ToList()
        };
    }

    public static League ToEntity(this CreateLeagueDto dto)
    {
        return new League
        {
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }

    public static void UpdateEntity(this UpdateLeagueDto dto, League league)
    {
        if (dto.Name != null) league.Name = dto.Name;
        league.Description = dto.Description;
        if (dto.StartDate.HasValue) league.StartDate = dto.StartDate.Value;
        league.EndDate = dto.EndDate;
        if (dto.IsActive.HasValue) league.IsActive = dto.IsActive.Value;
    }
}
