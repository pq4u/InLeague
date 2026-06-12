namespace InLeague.Application.Features.Karts.Interfaces;

public interface IKartRepository : IRepository<Kart>
{
    Task<IEnumerable<Kart>> GetAllAsync(bool? isActive);
    Task<bool> ExistsAsync(Guid id);
}
