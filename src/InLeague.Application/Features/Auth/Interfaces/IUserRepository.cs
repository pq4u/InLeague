using System;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Auth.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<Role?> GetRoleByNameAsync(string name);
}
