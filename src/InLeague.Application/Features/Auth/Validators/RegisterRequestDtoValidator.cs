using FluentValidation;

namespace InLeague.Application.Features.Auth.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidlowy format adresu email")
            .MaximumLength(256).WithMessage("Email moze miec maksymalnie 256 znakow");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Haslo jest wymagane")
            .MinimumLength(8).WithMessage("Haslo musi miec co najmniej 8 znakow")
            .Matches("[A-Z]").WithMessage("Haslo musi zawierac co najmniej jedna wielka litere")
            .Matches("[0-9]").WithMessage("Haslo musi zawierac co najmniej jedna cyfre");

        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithMessage("Imie moze miec maksymalnie 50 znakow")
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Nazwisko moze miec maksymalnie 50 znakow")
            .When(x => x.LastName is not null);
    }
}
