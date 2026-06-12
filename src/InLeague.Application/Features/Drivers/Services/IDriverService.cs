namespace InLeague.Application.Features.Drivers.Services;

public interface IDriverService
{
    Task<IEnumerable<DriverDto>> GetAllAsync();
    Task<DriverDto?> GetByIdAsync(Guid id);
    Task<DriverDto> CreateAsync(CreateDriverDto dto);
    Task<DriverDto?> UpdateAsync(Guid id, UpdateDriverDto dto);
    Task<bool> DeleteAsync(Guid id);
}
