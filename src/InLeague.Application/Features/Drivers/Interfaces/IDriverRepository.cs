namespace InLeague.Application.Features.Drivers.Interfaces;

public interface IDriverRepository : IRepository<Driver>
{
    Task<bool> ExistsAsync(Guid id);
}
