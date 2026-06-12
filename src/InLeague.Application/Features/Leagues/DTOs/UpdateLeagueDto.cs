namespace InLeague.Application.Features.Leagues.DTOs;

public class UpdateLeagueDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? IsActive { get; set; }
}
