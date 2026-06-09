using InLeague.Application.Features.RaceResults.DTOs;
using InLeague.Domain.Features.Races.Enums;
using Xunit;
using FluentValidation.TestHelper;
using System;

namespace InLeague.Tests.Validators;

public class RaceResultValidatorTests
{
    private readonly CreateRaceResultDtoValidator _validator;

    public RaceResultValidatorTests()
    {
        _validator = new CreateRaceResultDtoValidator();
    }

    [Fact]
    public void CreateRaceResultDtoValidator_Valid_Passes()
    {
        var model = new CreateRaceResultDto 
        { 
            DriverId = Guid.NewGuid(), 
            KartId = Guid.NewGuid(),
            LapTimeMs = 45000,
            TotalTimeMs = 600000,
            StartingPosition = 1,
            LapsCompleted = 10,
            Status = ResultStatus.Finished
        };
        _validator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateRaceResultDtoValidator_InvalidLapTime_Fails()
    {
        var model = new CreateRaceResultDto 
        { 
            DriverId = Guid.NewGuid(), 
            KartId = Guid.NewGuid(),
            StartingPosition = 1,
            LapTimeMs = -1 
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.LapTimeMs);
    }
}
