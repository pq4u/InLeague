using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Leagues.Interfaces;

public interface ILeagueRepository : IRepository<League>
{
    Task<IEnumerable<League>> GetAllWithRacesAsync();
    Task<League?> GetByIdWithRacesAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
