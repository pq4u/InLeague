using InLeague.Data;

namespace InLeague.Infrastructure.Features.RaceResults.Repositories;

public class RaceResultRepository : IRaceResultRepository
{
    private readonly ApplicationDbContext _context;

    public RaceResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RaceResult?> GetByIdAsync(Guid id)
        => await _context.RaceResults.FindAsync(id);

    public async Task<RaceResult?> GetByIdWithDetailsAsync(Guid id)
        => await _context.RaceResults
            .Include(r => r.Driver)
            .Include(r => r.Kart)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<RaceResult>> GetAllAsync()
        => await _context.RaceResults.ToListAsync();

    public async Task<IEnumerable<RaceResult>> GetByRaceIdAsync(Guid raceId)
        => await _context.RaceResults
            .Include(r => r.Driver)
            .Include(r => r.Kart)
            .Where(r => r.RaceId == raceId)
            .OrderBy(r => r.FinishingPosition ?? int.MaxValue)
            .ToListAsync();

    public async Task<bool> ResultExistsAsync(Guid raceId, Guid driverId)
        => await _context.RaceResults.AnyAsync(r => r.RaceId == raceId && r.DriverId == driverId);

    public async Task<RaceResult> CreateAsync(RaceResult entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.RaceResults.Add(entity);
        return entity;
    }

    public async Task<RaceResult> UpdateAsync(RaceResult entity)
    {
        _context.RaceResults.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(RaceResult entity)
    {
        _context.RaceResults.Remove(entity);
    }
}
