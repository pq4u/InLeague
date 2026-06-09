using InLeague.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Infrastructure.Features.Races.Repositories;

public class RaceRepository : IRaceRepository
{
    private readonly ApplicationDbContext _context;

    public RaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Race?> GetByIdAsync(Guid id)
        => await _context.Races.FindAsync(id);

    public async Task<IEnumerable<Race>> GetAllAsync()
        => await _context.Races.ToListAsync();

    public async Task<IEnumerable<Race>> GetByLeagueIdAsync(Guid leagueId)
        => await _context.Races
            .Where(r => r.LeagueId == leagueId)
            .OrderBy(r => r.RaceDate)
            .ToListAsync();

    public async Task<Race?> GetByIdWithResultsAsync(Guid id)
        => await _context.Races
            .Include(r => r.Results)
                .ThenInclude(rr => rr.Driver)
            .Include(r => r.Results)
                .ThenInclude(rr => rr.Kart)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Race> CreateAsync(Race entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.Races.Add(entity);
        return entity;
    }

    public async Task<Race> UpdateAsync(Race entity)
    {
        _context.Races.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(Race entity)
    {
        _context.Races.Remove(entity);
    }
}
