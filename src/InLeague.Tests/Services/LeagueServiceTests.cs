using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Leagues.DTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InLeague.Tests.Services;

public class LeagueServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<ILeagueRepository> _mockRepo;
    private readonly LeagueService _service;

    public LeagueServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<ILeagueRepository>();
        _mockUow.Setup(u => u.Leagues).Returns(_mockRepo.Object);
        _service = new LeagueService(_mockUow.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsLeagues()
    {
        var leagues = new List<League> { new() { Id = Guid.NewGuid(), Name = "L1" } };
        _mockRepo.Setup(r => r.GetAllWithRacesAsync()).ReturnsAsync(leagues);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedLeague()
    {
        var dto = new CreateLeagueDto { Name = "L1" };
        var league = new League { Id = Guid.NewGuid(), Name = "L1" };
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<League>())).ReturnsAsync(league);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NoRaces_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var league = new League { Id = id, Races = new List<Race>() };
        _mockRepo.Setup(r => r.GetByIdWithRacesAsync(id)).ReturnsAsync(league);

        var result = await _service.DeleteAsync(id);

        Assert.True(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_HasRaces_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        var league = new League { Id = id, Races = new List<Race> { new Race() } };
        _mockRepo.Setup(r => r.GetByIdWithRacesAsync(id)).ReturnsAsync(league);

        var result = await _service.DeleteAsync(id);

        Assert.False(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
