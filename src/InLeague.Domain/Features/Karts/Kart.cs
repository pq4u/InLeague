using System;
using System.Collections.Generic;

namespace InLeague.Domain.Features.Karts;

public class Kart
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;         // max 10 znaków, wymagane, unikalny
    public string? Model { get; set; }                         // max 100 znaków, opcjonalne
    public string? Category { get; set; }                      // max 50 znaków, np. "Senior", "Junior"
    public bool IsActive { get; set; }                         // domyślnie true

    // Relacje
    public ICollection<RaceResult> Results { get; set; } = new List<RaceResult>();
}
