namespace InLeague.Domain.Features.Drivers;

public class Driver
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RacingNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<RaceResult> Results { get; set; } = new List<RaceResult>();
    public User? User { get; set; }
}
