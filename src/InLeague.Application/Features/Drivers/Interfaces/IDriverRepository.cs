using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Drivers.Interfaces;

public interface IDriverRepository : IRepository<Driver>
{
    Task<bool> ExistsAsync(Guid id);
}
