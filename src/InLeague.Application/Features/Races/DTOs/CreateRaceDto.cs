using System;

namespace InLeague.Application.Features.Races.DTOs;

public class CreateRaceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime RaceDate { get; set; }
    public int NumberOfLaps { get; set; }
    public string? Notes { get; set; }
}
