using InLeague.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLeague.Infrastructure.Features.Karts.Repositories;

public class KartRepository : IKartRepository
{
    private readonly ApplicationDbContext _context;

    public KartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Kart?> GetByIdAsync(Guid id)
        => await _context.Karts.FindAsync(id);

    public async Task<IEnumerable<Kart>> GetAllAsync()
        => await _context.Karts.ToListAsync();

    public async Task<IEnumerable<Kart>> GetAllAsync(bool? isActive)
    {
        var query = _context.Karts.AsQueryable();
        if (isActive.HasValue)
            query = query.Where(k => k.IsActive == isActive.Value);
        return await query.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await _context.Karts.AnyAsync(k => k.Id == id);

    public async Task<Kart> CreateAsync(Kart entity)
    {
        entity.Id = Guid.NewGuid();
        _context.Karts.Add(entity);
        return entity;
    }

    public async Task<Kart> UpdateAsync(Kart entity)
    {
        _context.Karts.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(Kart entity)
    {
        _context.Karts.Remove(entity);
    }
}
