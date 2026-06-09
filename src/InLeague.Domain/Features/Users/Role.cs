using System;
using System.Collections.Generic;

namespace InLeague.Domain.Features.Users;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;           // "Admin" lub "User"

    // Relacje
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
