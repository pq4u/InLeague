namespace InLeague.Domain.Features.Karts;

public class Kart
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; }

    public ICollection<RaceResult> Results { get; set; } = new List<RaceResult>();
}
