using System;
using InLeague.Domain.Features.Races.Enums;

namespace InLeague.Domain.Features.Races;

public class RaceResult
{
    public Guid Id { get; set; }
    public Guid RaceId { get; set; }                           // FK → Race
    public Guid DriverId { get; set; }                         // FK → Driver
    public Guid KartId { get; set; }                           // FK → Kart
    public long LapTimeMs { get; set; }                        // czas najszybszego okrążenia w ms
    public long TotalTimeMs { get; set; }                      // łączny czas przejazdu w ms
    public int StartingPosition { get; set; }                  // pozycja startowa, min 1
    public int? FinishingPosition { get; set; }                // null = DNF
    public int LapsCompleted { get; set; }                     // ukończone okrążenia
    public ResultStatus Status { get; set; }                   // enum
    public string? Notes { get; set; }                         // max 200 znaków
    public DateTime CreatedAt { get; set; }

    // Relacje
    public Race Race { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public Kart Kart { get; set; } = null!;
}
