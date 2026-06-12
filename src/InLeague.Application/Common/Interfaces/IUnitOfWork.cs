namespace InLeague.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    ILeagueRepository Leagues { get; }
    IRaceRepository Races { get; }
    IDriverRepository Drivers { get; }
    IKartRepository Karts { get; }
    IRaceResultRepository RaceResults { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
