namespace InLeague.Application.Features.RaceResults.Mappings;

public static class RaceResultMappingExtensions
{
    public static RaceResultDto ToDto(this RaceResult result)
    {
        return new RaceResultDto
        {
            Id = result.Id,
            RaceId = result.RaceId,
            DriverId = result.DriverId,
            DriverName = result.Driver != null ? $"{result.Driver.FirstName} {result.Driver.LastName}" : string.Empty,
            KartId = result.KartId,
            KartNumber = result.Kart?.Number ?? string.Empty,
            LapTimeMs = result.LapTimeMs,
            TotalTimeMs = result.TotalTimeMs,
            StartingPosition = result.StartingPosition,
            FinishingPosition = result.FinishingPosition,
            LapsCompleted = result.LapsCompleted,
            Status = result.Status.ToString(),
            Notes = result.Notes
        };
    }

    public static RaceResult ToEntity(this CreateRaceResultDto dto, Guid raceId)
    {
        return new RaceResult
        {
            RaceId = raceId,
            DriverId = dto.DriverId,
            KartId = dto.KartId,
            LapTimeMs = dto.LapTimeMs,
            TotalTimeMs = dto.TotalTimeMs,
            StartingPosition = dto.StartingPosition,
            FinishingPosition = dto.FinishingPosition,
            LapsCompleted = dto.LapsCompleted,
            Status = dto.Status,
            Notes = dto.Notes
        };
    }

    public static void UpdateEntity(this UpdateRaceResultDto dto, RaceResult result)
    {
        if (dto.KartId.HasValue) result.KartId = dto.KartId.Value;
        if (dto.LapTimeMs.HasValue) result.LapTimeMs = dto.LapTimeMs.Value;
        if (dto.TotalTimeMs.HasValue) result.TotalTimeMs = dto.TotalTimeMs.Value;
        if (dto.StartingPosition.HasValue) result.StartingPosition = dto.StartingPosition.Value;
        result.FinishingPosition = dto.FinishingPosition;
        if (dto.LapsCompleted.HasValue) result.LapsCompleted = dto.LapsCompleted.Value;
        if (dto.Status.HasValue) result.Status = dto.Status.Value;
        result.Notes = dto.Notes;
    }
}
