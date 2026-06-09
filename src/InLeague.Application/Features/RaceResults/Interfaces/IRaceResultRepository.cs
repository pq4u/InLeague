using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Application.Features.RaceResults.Interfaces;

public interface IRaceResultRepository : IRepository<RaceResult>
{
    Task<IEnumerable<RaceResult>> GetByRaceIdAsync(Guid raceId);
    Task<bool> ResultExistsAsync(Guid raceId, Guid driverId);
    Task<RaceResult?> GetByIdWithDetailsAsync(Guid id);
}
