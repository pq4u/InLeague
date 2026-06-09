using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.RaceResults.DTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InLeague.Tests.Services;

public class RaceResultServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IRaceResultRepository> _mockResultRepo;
    private readonly Mock<IRaceRepository> _mockRaceRepo;
    private readonly Mock<IDriverRepository> _mockDriverRepo;
    private readonly Mock<IKartRepository> _mockKartRepo;
    private readonly RaceResultService _service;

    public RaceResultServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockResultRepo = new Mock<IRaceResultRepository>();
        _mockRaceRepo = new Mock<IRaceRepository>();
        _mockDriverRepo = new Mock<IDriverRepository>();
        _mockKartRepo = new Mock<IKartRepository>();

        _mockUow.Setup(u => u.RaceResults).Returns(_mockResultRepo.Object);
        _mockUow.Setup(u => u.Races).Returns(_mockRaceRepo.Object);
        _mockUow.Setup(u => u.Drivers).Returns(_mockDriverRepo.Object);
        _mockUow.Setup(u => u.Karts).Returns(_mockKartRepo.Object);

        _service = new RaceResultService(_mockUow.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsResult()
    {
        var raceId = Guid.NewGuid();
        var dto = new CreateRaceResultDto { DriverId = Guid.NewGuid(), KartId = Guid.NewGuid() };
        
        _mockRaceRepo.Setup(r => r.GetByIdAsync(raceId)).ReturnsAsync(new Race());
        _mockDriverRepo.Setup(r => r.ExistsAsync(dto.DriverId)).ReturnsAsync(true);
        _mockKartRepo.Setup(r => r.ExistsAsync(dto.KartId)).ReturnsAsync(true);
        _mockResultRepo.Setup(r => r.ResultExistsAsync(raceId, dto.DriverId)).ReturnsAsync(false);
        
        var entity = new RaceResult { Id = Guid.NewGuid() };
        _mockResultRepo.Setup(r => r.CreateAsync(It.IsAny<RaceResult>())).ReturnsAsync(entity);
        _mockResultRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>())).ReturnsAsync(entity);

        var result = await _service.CreateAsync(raceId, dto);

        Assert.NotNull(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
