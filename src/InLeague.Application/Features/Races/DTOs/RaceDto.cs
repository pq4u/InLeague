using System;
using System.Collections.Generic;
using InLeague.Application.Features.RaceResults.DTOs;

namespace InLeague.Application.Features.Races.DTOs;

public class RaceDto
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime RaceDate { get; set; }
    public int NumberOfLaps { get; set; }
    public string? Notes { get; set; }
    public int ResultCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RaceResultDto>? Results { get; set; }
}
