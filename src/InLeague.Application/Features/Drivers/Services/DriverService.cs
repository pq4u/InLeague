using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Drivers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Drivers.Services;

public class DriverService : IDriverService
{
    private readonly IUnitOfWork _uow;

    public DriverService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<DriverDto>> GetAllAsync()
    {
        var drivers = await _uow.Drivers.GetAllAsync();
        return drivers.Select(d => d.ToDto());
    }

    public async Task<DriverDto?> GetByIdAsync(Guid id)
    {
        var driver = await _uow.Drivers.GetByIdAsync(id);
        return driver?.ToDto();
    }

    public async Task<DriverDto> CreateAsync(CreateDriverDto dto)
    {
        var driver = dto.ToEntity();
        await _uow.Drivers.CreateAsync(driver);
        await _uow.SaveChangesAsync();
        return driver.ToDto();
    }

    public async Task<DriverDto?> UpdateAsync(Guid id, UpdateDriverDto dto)
    {
        var driver = await _uow.Drivers.GetByIdAsync(id);
        if (driver is null) return null;

        dto.UpdateEntity(driver);
        await _uow.Drivers.UpdateAsync(driver);
        await _uow.SaveChangesAsync();
        return driver.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var driver = await _uow.Drivers.GetByIdAsync(id);
        if (driver is null) return false;

        await _uow.Drivers.DeleteAsync(driver);
        await _uow.SaveChangesAsync();
        return true;
    }
}
