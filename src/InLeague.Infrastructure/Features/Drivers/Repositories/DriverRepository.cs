using InLeague.Data;

namespace InLeague.Infrastructure.Features.Drivers.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly ApplicationDbContext _context;

    public DriverRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Driver?> GetByIdAsync(Guid id)
        => await _context.Drivers.FindAsync(id);

    public async Task<IEnumerable<Driver>> GetAllAsync()
        => await _context.Drivers.OrderBy(d => d.LastName).ToListAsync();

    public async Task<bool> ExistsAsync(Guid id)
        => await _context.Drivers.AnyAsync(d => d.Id == id);

    public async Task<Driver> CreateAsync(Driver entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _context.Drivers.Add(entity);
        return entity;
    }

    public async Task<Driver> UpdateAsync(Driver entity)
    {
        _context.Drivers.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(Driver entity)
    {
        _context.Drivers.Remove(entity);
    }
}
