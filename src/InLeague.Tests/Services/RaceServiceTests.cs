using Moq;

namespace InLeague.Tests.Services;

public class RaceServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IRaceRepository> _mockRaceRepo;
    private readonly Mock<ILeagueRepository> _mockLeagueRepo;
    private readonly RaceService _service;

    public RaceServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockRaceRepo = new Mock<IRaceRepository>();
        _mockLeagueRepo = new Mock<ILeagueRepository>();
        
        _mockUow.Setup(u => u.Races).Returns(_mockRaceRepo.Object);
        _mockUow.Setup(u => u.Leagues).Returns(_mockLeagueRepo.Object);
        
        _service = new RaceService(_mockUow.Object);
    }

    [Fact]
    public async Task CreateAsync_LeagueExists_ReturnsCreatedRace()
    {
        var leagueId = Guid.NewGuid();
        var dto = new CreateRaceDto { Name = "R1" };
        var race = new Race { Id = Guid.NewGuid(), Name = "R1" };
        
        _mockLeagueRepo.Setup(r => r.ExistsAsync(leagueId)).ReturnsAsync(true);
        _mockRaceRepo.Setup(r => r.CreateAsync(It.IsAny<Race>())).ReturnsAsync(race);

        var result = await _service.CreateAsync(leagueId, dto);

        Assert.NotNull(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_LeagueDoesNotExist_ReturnsNull()
    {
        var leagueId = Guid.NewGuid();
        var dto = new CreateRaceDto { Name = "R1" };
        
        _mockLeagueRepo.Setup(r => r.ExistsAsync(leagueId)).ReturnsAsync(false);

        var result = await _service.CreateAsync(leagueId, dto);

        Assert.Null(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
