using System;

namespace InLeague.Application.Features.Leagues.DTOs;

public class CreateLeagueDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
