namespace InLeague.Application.Features.Drivers.Mappings;

public static class DriverMappingExtensions
{
    public static DriverDto ToDto(this Driver driver)
    {
        return new DriverDto
        {
            Id = driver.Id,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            FullName = $"{driver.FirstName} {driver.LastName}",
            RacingNumber = driver.RacingNumber,
            DateOfBirth = driver.DateOfBirth,
            CreatedAt = driver.CreatedAt
        };
    }

    public static Driver ToEntity(this CreateDriverDto dto)
    {
        return new Driver
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            RacingNumber = dto.RacingNumber,
            DateOfBirth = dto.DateOfBirth
        };
    }

    public static void UpdateEntity(this UpdateDriverDto dto, Driver driver)
    {
        if (dto.FirstName != null) driver.FirstName = dto.FirstName;
        if (dto.LastName != null) driver.LastName = dto.LastName;
        driver.RacingNumber = dto.RacingNumber;
        driver.DateOfBirth = dto.DateOfBirth;
    }
}
