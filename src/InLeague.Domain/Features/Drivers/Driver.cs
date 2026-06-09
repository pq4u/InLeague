using System;
using System.Collections.Generic;

namespace InLeague.Domain.Features.Drivers;

public class Driver
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;      // max 50 znaków, wymagane
    public string LastName { get; set; } = string.Empty;       // max 50 znaków, wymagane
    public string? RacingNumber { get; set; }                  // max 10 znaków, unikalny w ramach ligi (opcjonalne)
    public DateOnly? DateOfBirth { get; set; }                 // opcjonalne
    public DateTime CreatedAt { get; set; }

    // Relacje
    public ICollection<RaceResult> Results { get; set; } = new List<RaceResult>();
    public User? User { get; set; }                            // opcjonalne powiązanie z kontem
}
