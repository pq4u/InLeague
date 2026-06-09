using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Races.Interfaces;

public interface IRaceRepository : IRepository<Race>
{
    Task<IEnumerable<Race>> GetByLeagueIdAsync(Guid leagueId);
    Task<Race?> GetByIdWithResultsAsync(Guid id);
}
