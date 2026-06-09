using InLeague.Application.Common.Interfaces;
using InLeague.Application.Features.Drivers.DTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InLeague.Tests.Services;

public class DriverServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IDriverRepository> _mockRepo;
    private readonly DriverService _service;

    public DriverServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IDriverRepository>();
        _mockUow.Setup(u => u.Drivers).Returns(_mockRepo.Object);
        _service = new DriverService(_mockUow.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsDrivers()
    {
        var drivers = new List<Driver>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(drivers);

        var result = await _service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDriver()
    {
        var id = Guid.NewGuid();
        var driver = new Driver { Id = id, FirstName = "John", LastName = "Doe" };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(driver);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Driver?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedDriver()
    {
        var createDto = new CreateDriverDto { FirstName = "John", LastName = "Doe" };
        var driver = new Driver { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Driver>())).ReturnsAsync(driver);

        var result = await _service.CreateAsync(createDto);

        Assert.NotNull(result);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Driver>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_ReturnsUpdatedDriver()
    {
        var id = Guid.NewGuid();
        var updateDto = new UpdateDriverDto { FirstName = "Johnny" };
        var driver = new Driver { Id = id, FirstName = "John", LastName = "Doe" };
        
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Driver>())).ReturnsAsync((Driver d) => d);

        var result = await _service.UpdateAsync(id, updateDto);

        Assert.NotNull(result);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Driver>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var driver = new Driver { Id = id };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(driver);

        var result = await _service.DeleteAsync(id);

        Assert.True(result);
        _mockRepo.Verify(r => r.DeleteAsync(driver), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Driver?)null);

        var result = await _service.DeleteAsync(id);

        Assert.False(result);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Driver>()), Times.Never);
    }
}
