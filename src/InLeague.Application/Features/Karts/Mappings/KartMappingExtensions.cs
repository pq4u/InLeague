namespace InLeague.Application.Features.Karts.Mappings;

public static class KartMappingExtensions
{
    public static KartDto ToDto(this Kart kart)
    {
        return new KartDto
        {
            Id = kart.Id,
            Number = kart.Number,
            Model = kart.Model,
            Category = kart.Category,
            IsActive = kart.IsActive
        };
    }

    public static Kart ToEntity(this CreateKartDto dto)
    {
        return new Kart
        {
            Number = dto.Number,
            Model = dto.Model,
            Category = dto.Category
        };
    }

    public static void UpdateEntity(this UpdateKartDto dto, Kart kart)
    {
        if (dto.Number != null) kart.Number = dto.Number;
        kart.Model = dto.Model;
        kart.Category = dto.Category;
        if (dto.IsActive.HasValue) kart.IsActive = dto.IsActive.Value;
    }
}
