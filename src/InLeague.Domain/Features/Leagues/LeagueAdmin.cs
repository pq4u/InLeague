using System;

namespace InLeague.Domain.Features.Leagues;

public class LeagueAdmin
{
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; }

    public League League { get; set; } = null!;
    public User User { get; set; } = null!;
}
