using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Karts.DTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InLeague.Tests.Services;

public class KartServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IKartRepository> _mockRepo;
    private readonly KartService _service;

    public KartServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IKartRepository>();
        _mockUow.Setup(u => u.Karts).Returns(_mockRepo.Object);
        _service = new KartService(_mockUow.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsKarts()
    {
        var karts = new List<Kart> { new() { Id = Guid.NewGuid(), Number = "01" } };
        _mockRepo.Setup(r => r.GetAllAsync(null)).ReturnsAsync(karts);

        var result = await _service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsKart()
    {
        var id = Guid.NewGuid();
        var kart = new Kart { Id = id, Number = "01" };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(kart);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedKart()
    {
        var dto = new CreateKartDto { Number = "01" };
        var kart = new Kart { Id = Guid.NewGuid(), Number = "01" };
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Kart>())).ReturnsAsync(kart);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_ReturnsUpdatedKart()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateKartDto { Number = "02" };
        var kart = new Kart { Id = id, Number = "01" };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(kart);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Kart>())).ReturnsAsync(kart);

        var result = await _service.UpdateAsync(id, dto);

        Assert.NotNull(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var kart = new Kart { Id = id };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(kart);

        var result = await _service.DeleteAsync(id);

        Assert.True(result);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
