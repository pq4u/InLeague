using System;
using System.Collections.Generic;
using InLeague.Application.Features.Races.DTOs;

namespace InLeague.Application.Features.Leagues.DTOs;

public class LeagueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int RaceCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RaceDto>? Races { get; set; }
}
