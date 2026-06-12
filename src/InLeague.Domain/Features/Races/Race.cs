namespace InLeague.Domain.Features.Races;

public class Race
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime RaceDate { get; set; } 
    public int NumberOfLaps { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public League League { get; set; } = null!;
    public ICollection<RaceResult> Results { get; set; } = new List<RaceResult>();
}
