using System;

namespace InLeague.Application.Features.Drivers.DTOs;

public class UpdateDriverDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RacingNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
