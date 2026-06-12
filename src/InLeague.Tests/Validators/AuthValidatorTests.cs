using FluentValidation.TestHelper;

namespace InLeague.Tests.Validators;

public class AuthValidatorTests
{
    private readonly LoginRequestDtoValidator _loginValidator;
    private readonly RegisterRequestDtoValidator _registerValidator;

    public AuthValidatorTests()
    {
        _loginValidator = new LoginRequestDtoValidator();
        _registerValidator = new RegisterRequestDtoValidator();
    }

    [Fact]
    public void LoginValidator_ValidData_Passes()
    {
        var model = new LoginRequestDto { Email = "test@test.com", Password = "Password123" };
        var result = _loginValidator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void RegisterValidator_ValidData_Passes()
    {
        var model = new RegisterRequestDto 
        { 
            Email = "test@test.com", 
            Password = "Password123", 
            FirstName = "John", 
            LastName = "Doe" 
        };
        var result = _registerValidator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void RegisterValidator_InvalidEmail_Fails()
    {
        var model = new RegisterRequestDto { Email = "invalid", Password = "Password123" };
        var result = _registerValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void RegisterValidator_WeakPassword_Fails()
    {
        var model = new RegisterRequestDto { Email = "test@test.com", Password = "short" };
        var result = _registerValidator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
