namespace InLeague.Application.Features.Karts.DTOs;

public class CreateKartDto
{
    public string Number { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Category { get; set; }
}
