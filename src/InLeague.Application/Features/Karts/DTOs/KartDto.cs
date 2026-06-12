namespace InLeague.Application.Features.Karts.DTOs;

public class KartDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; }
}
