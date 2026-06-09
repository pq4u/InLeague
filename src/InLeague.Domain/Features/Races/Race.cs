using System;
using System.Collections.Generic;

namespace InLeague.Domain.Features.Races;

public class Race
{
    public Guid Id { get; set; }
    public Guid LeagueId { get; set; }                         // FK → League
    public string Name { get; set; } = string.Empty;           // max 100 znaków, wymagane
    public string? Location { get; set; }                      // max 200 znaków, opcjonalne
    public DateTime RaceDate { get; set; }                     // data i godzina wyścigu
    public int NumberOfLaps { get; set; }                      // liczba okrążeń, min 1
    public string? Notes { get; set; }                         // max 1000 znaków, opcjonalne
    public DateTime CreatedAt { get; set; }

    // Relacje
    public League League { get; set; } = null!;
    public ICollection<RaceResult> Results { get; set; } = new List<RaceResult>();
}
