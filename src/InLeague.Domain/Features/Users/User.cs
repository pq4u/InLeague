using System;
using System.Collections.Generic;

namespace InLeague.Domain.Features.Users;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;          // max 256 znaków, unikalny, wymagane
    public string PasswordHash { get; set; } = string.Empty;   // BCrypt hash, nigdy nie zwracać w API
    public string? FirstName { get; set; }                     // max 50 znaków
    public string? LastName { get; set; }                      // max 50 znaków
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }                         // domyślnie true

    // Relacje
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public Driver? Driver { get; set; }                        // opcjonalne powiązanie z zawodnikiem
    public Guid? DriverId { get; set; }
}
