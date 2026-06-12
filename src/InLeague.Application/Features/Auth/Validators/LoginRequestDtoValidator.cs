using FluentValidation;

namespace InLeague.Application.Features.Auth.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidlowy format adresu email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Haslo jest wymagane");
    }
}
