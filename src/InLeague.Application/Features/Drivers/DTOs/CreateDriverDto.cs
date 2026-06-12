namespace InLeague.Application.Features.Drivers.DTOs;

public class CreateDriverDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RacingNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
