namespace InLeague.Application.Features.RaceResults.DTOs;

public class CreateRaceResultDto
{
    public Guid DriverId { get; set; }
    public Guid KartId { get; set; }
    public long LapTimeMs { get; set; }
    public long TotalTimeMs { get; set; }
    public int StartingPosition { get; set; }
    public int? FinishingPosition { get; set; }
    public int LapsCompleted { get; set; }
    public ResultStatus Status { get; set; }
    public string? Notes { get; set; }
}
