using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Karts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InLeague.Application.Features.Karts.Services;

public class KartService : IKartService
{
    private readonly IUnitOfWork _uow;

    public KartService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<KartDto>> GetAllAsync(bool? isActive = null)
    {
        var karts = await _uow.Karts.GetAllAsync(isActive);
        return karts.Select(k => k.ToDto());
    }

    public async Task<KartDto?> GetByIdAsync(Guid id)
    {
        var kart = await _uow.Karts.GetByIdAsync(id);
        return kart?.ToDto();
    }

    public async Task<KartDto> CreateAsync(CreateKartDto dto)
    {
        var kart = dto.ToEntity();
        kart.IsActive = true;
        await _uow.Karts.CreateAsync(kart);
        await _uow.SaveChangesAsync();
        return kart.ToDto();
    }

    public async Task<KartDto?> UpdateAsync(Guid id, UpdateKartDto dto)
    {
        var kart = await _uow.Karts.GetByIdAsync(id);
        if (kart is null) return null;

        dto.UpdateEntity(kart);
        await _uow.Karts.UpdateAsync(kart);
        await _uow.SaveChangesAsync();
        return kart.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var kart = await _uow.Karts.GetByIdAsync(id);
        if (kart is null) return false;

        await _uow.Karts.DeleteAsync(kart);
        await _uow.SaveChangesAsync();
        return true;
    }
}
