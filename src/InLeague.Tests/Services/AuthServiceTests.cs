using Microsoft.Extensions.Configuration;
using Moq;

namespace InLeague.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockConfig = new Mock<IConfiguration>();

        _mockUow.Setup(u => u.Users).Returns(_mockUserRepo.Object);

        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyForTestingPurposesOnly123!");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("Issuer");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("Audience");
        _mockConfig.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("60");

        _service = new AuthService(_mockUow.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var password = "Password123!";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            UserRoles = new List<UserRole>()
        };

        _mockUserRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequestDto { Email = user.Email, Password = password });

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
