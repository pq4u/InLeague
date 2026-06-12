namespace InLeague.Application.Features.Auth.Mappings;

public static class UserMappingExtensions
{
    public static UserInfoDto ToDto(this User user)
    {
        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>(),
            DriverId = user.Driver?.Id
        };
    }
}
