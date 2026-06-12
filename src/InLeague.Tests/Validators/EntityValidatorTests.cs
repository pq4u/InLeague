using FluentValidation.TestHelper;

namespace InLeague.Tests.Validators;

public class EntityValidatorTests
{
    [Fact]
    public void CreateDriverDtoValidator_Valid_Passes()
    {
        var validator = new CreateDriverDtoValidator();
        var model = new CreateDriverDto { FirstName = "John", LastName = "Doe" };
        validator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateKartDtoValidator_Valid_Passes()
    {
        var validator = new CreateKartDtoValidator();
        var model = new CreateKartDto { Number = "10" };
        validator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateLeagueDtoValidator_Valid_Passes()
    {
        var validator = new CreateLeagueDtoValidator();
        var model = new CreateLeagueDto { Name = "Super League", StartDate = System.DateOnly.FromDateTime(System.DateTime.UtcNow) };
        validator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateRaceDtoValidator_Valid_Passes()
    {
        var validator = new CreateRaceDtoValidator();
        var model = new CreateRaceDto { Name = "Race 1", RaceDate = System.DateTime.UtcNow.AddDays(1), NumberOfLaps = 10 };
        validator.TestValidate(model).ShouldNotHaveAnyValidationErrors();
    }
}
