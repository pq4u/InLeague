namespace InLeague.Domain.Features.Races;

public class RaceResult
{
    public Guid Id { get; set; }
    public Guid RaceId { get; set; }
    public Guid DriverId { get; set; }
    public Guid KartId { get; set; }
    public long LapTimeMs { get; set; }
    public long TotalTimeMs { get; set; }
    public int StartingPosition { get; set; } 
    public int? FinishingPosition { get; set; }
    public int LapsCompleted { get; set; } 
    public ResultStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public Race Race { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public Kart Kart { get; set; } = null!;
}
