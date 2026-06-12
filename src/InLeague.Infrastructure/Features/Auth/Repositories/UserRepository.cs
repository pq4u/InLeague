using InLeague.Data;

namespace InLeague.Infrastructure.Features.Auth.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());

    public async Task<Role?> GetRoleByNameAsync(string name)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);

    public async Task<User> CreateAsync(User entity)
    {
        _context.Users.Add(entity);
        return entity;
    }

    public async Task<User> UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
    }
}
