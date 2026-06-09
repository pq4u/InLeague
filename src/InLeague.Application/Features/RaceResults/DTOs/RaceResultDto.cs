using System;
using InLeague.Domain.Features.Races.Enums;

namespace InLeague.Application.Features.RaceResults.DTOs;

public class RaceResultDto
{
    public Guid Id { get; set; }
    public Guid RaceId { get; set; }
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public Guid KartId { get; set; }
    public string KartNumber { get; set; } = string.Empty;
    public long LapTimeMs { get; set; }
    public long TotalTimeMs { get; set; }
    public int StartingPosition { get; set; }
    public int? FinishingPosition { get; set; }
    public int LapsCompleted { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
