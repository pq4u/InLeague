namespace InLeague.Application.Features.RaceResults.Services;

public class RaceResultService : IRaceResultService
{
    private readonly IUnitOfWork _uow;

    public RaceResultService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<RaceResultDto>> GetByRaceIdAsync(Guid raceId)
    {
        var results = await _uow.RaceResults.GetByRaceIdAsync(raceId);
        return results.Select(r => r.ToDto());
    }

    public async Task<RaceResultDto?> GetByIdAsync(Guid raceId, Guid id)
    {
        var result = await _uow.RaceResults.GetByIdWithDetailsAsync(id);
        if (result is null || result.RaceId != raceId) return null;
        return result.ToDto();
    }

    public async Task<RaceResultDto?> CreateAsync(Guid raceId, CreateRaceResultDto dto)
    {
        var race = await _uow.Races.GetByIdAsync(raceId);
        if (race == null) return null;

        if (!await _uow.Drivers.ExistsAsync(dto.DriverId)) return null;
        if (!await _uow.Karts.ExistsAsync(dto.KartId)) return null;
        if (await _uow.RaceResults.ResultExistsAsync(raceId, dto.DriverId))
            throw new InvalidOperationException("Ten zawodnik ma już wynik w tym wyścigu");

        var result = dto.ToEntity(raceId);
        await _uow.RaceResults.CreateAsync(result);
        await _uow.SaveChangesAsync();

        var finalResult = await _uow.RaceResults.GetByIdWithDetailsAsync(result.Id);
        return finalResult?.ToDto();
    }

    public async Task<RaceResultDto?> UpdateAsync(Guid raceId, Guid id, UpdateRaceResultDto dto)
    {
        var result = await _uow.RaceResults.GetByIdAsync(id);
        if (result is null || result.RaceId != raceId) return null;

        if (dto.KartId.HasValue && !await _uow.Karts.ExistsAsync(dto.KartId.Value))
            return null;

        dto.UpdateEntity(result);
        await _uow.RaceResults.UpdateAsync(result);
        await _uow.SaveChangesAsync();
        return result.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid raceId, Guid id)
    {
        var result = await _uow.RaceResults.GetByIdAsync(id);
        if (result is null || result.RaceId != raceId) return false;

        await _uow.RaceResults.DeleteAsync(result);
        await _uow.SaveChangesAsync();
        return true;
    }
}
