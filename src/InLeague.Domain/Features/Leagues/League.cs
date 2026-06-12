namespace InLeague.Domain.Features.Leagues;

public class League
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public bool IsActive { get; set; }

    public ICollection<Race> Races { get; set; } = new List<Race>();
    public ICollection<LeagueAdmin> LeagueAdmins { get; set; } = new List<LeagueAdmin>();
}
