namespace InLeague.Application.Features.Karts.Services;

public interface IKartService
{
    Task<IEnumerable<KartDto>> GetAllAsync(bool? isActive = null);
    Task<KartDto?> GetByIdAsync(Guid id);
    Task<KartDto> CreateAsync(CreateKartDto dto);
    Task<KartDto?> UpdateAsync(Guid id, UpdateKartDto dto);
    Task<bool> DeleteAsync(Guid id);
}
