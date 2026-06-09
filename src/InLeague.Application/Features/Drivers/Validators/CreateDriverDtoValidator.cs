using FluentValidation;
using InLeague.Application.Features.Drivers.DTOs;

namespace InLeague.Application.Features.Drivers.Validators;

public class CreateDriverDtoValidator : AbstractValidator<CreateDriverDto>
{
    public CreateDriverDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imie jest wymagane")
            .Length(2, 50).WithMessage("Imie musi miec od 2 do 50 znakow");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane")
            .Length(2, 50).WithMessage("Nazwisko musi miec od 2 do 50 znakow");

        RuleFor(x => x.RacingNumber)
            .MaximumLength(10).WithMessage("Numer startowy moze miec maksymalnie 10 znakow")
            .When(x => x.RacingNumber is not null);
    }
}
