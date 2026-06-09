using System;
using System.Collections.Generic;

namespace InLeague.Domain.Features.Leagues;

public class League
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;           // max 100 znaków, wymagane
    public string? Description { get; set; }                   // max 500 znaków, opcjonalne
    public DateOnly StartDate { get; set; }                    // wymagane
    public DateOnly? EndDate { get; set; }                     // opcjonalne
    public DateTime CreatedAt { get; set; }                    // ustawiane automatycznie
    public bool IsActive { get; set; }                         // domyślnie true

    // Relacje
    public ICollection<Race> Races { get; set; } = new List<Race>();
    public ICollection<LeagueAdmin> LeagueAdmins { get; set; } = new List<LeagueAdmin>();
}
