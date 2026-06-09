using InLeague.Application.Common.Interfaces;
using InLeague.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InLeague.Infrastructure.Common;

/// <summary>
/// Implementacja Unit of Work. Wszystkie repozytoria wspoldziela ten sam
/// ApplicationDbContext (scoped), wiec operacje sa atomowe w ramach jednego requestu.
/// Wywolaj SaveChangesAsync() raz po zakonczeniu wszystkich operacji.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public ILeagueRepository Leagues { get; }
    public IRaceRepository Races { get; }
    public IDriverRepository Drivers { get; }
    public IKartRepository Karts { get; }
    public IRaceResultRepository RaceResults { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Leagues    = new LeagueRepository(context);
        Races      = new RaceRepository(context);
        Drivers    = new DriverRepository(context);
        Karts      = new KartRepository(context);
        RaceResults= new RaceResultRepository(context);
        Users      = new UserRepository(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public ValueTask DisposeAsync()
        => _context.DisposeAsync();
}
