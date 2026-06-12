namespace InLeague.Application.Features.Races.DTOs;

public class UpdateRaceDto
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public DateTime? RaceDate { get; set; }
    public int? NumberOfLaps { get; set; }
    public string? Notes { get; set; }
}
