using System;
using System.Threading;
using System.Threading.Tasks;

namespace InLeague.Application.Common.Interfaces;

/// <summary>
/// Unit of Work — grupuje wszystkie repozytoria i kontroluje
/// zapis zmian do bazy danych w ramach jednej transakcji.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    ILeagueRepository Leagues { get; }
    IRaceRepository Races { get; }
    IDriverRepository Drivers { get; }
    IKartRepository Karts { get; }
    IRaceResultRepository RaceResults { get; }
    IUserRepository Users { get; }

    /// <summary>Zapisuje wszystkie oczekujące zmiany do bazy danych.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
