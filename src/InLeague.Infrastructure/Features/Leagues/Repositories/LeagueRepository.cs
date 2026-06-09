using InLeague.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLeague.Infrastructure.Features.Leagues.Repositories;

public class LeagueRepository : ILeagueRepository
{
    private readonly ApplicationDbContext _context;

    public LeagueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<League?> GetByIdAsync(Guid id)
        => await _context.Leagues.FindAsync(id);

    public async Task<IEnumerable<League>> GetAllAsync()
        => await _context.Leagues.ToListAsync();

    public async Task<IEnumerable<League>> GetAllWithRacesAsync()
        => await _context.Leagues
            .Include(l => l.Races)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<League?> GetByIdWithRacesAsync(Guid id)
        => await _context.Leagues
            .Include(l => l.Races)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<bool> ExistsAsync(Guid id)
        => await _context.Leagues.AnyAsync(l => l.Id == id);

    public async Task<League> CreateAsync(League entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.Leagues.Add(entity);
        return entity;
    }

    public async Task<League> UpdateAsync(League entity)
    {
        _context.Leagues.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(League entity)
    {
        _context.Leagues.Remove(entity);
    }
}
