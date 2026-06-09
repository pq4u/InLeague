using FluentValidation;
using InLeague.Application.Features.Drivers.DTOs;

namespace InLeague.Application.Features.Drivers.Validators;

public class UpdateDriverDtoValidator : AbstractValidator<UpdateDriverDto>
{
    public UpdateDriverDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imie nie moze byc puste")
            .Length(2, 50).WithMessage("Imie musi miec od 2 do 50 znakow")
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko nie moze byc puste")
            .Length(2, 50).WithMessage("Nazwisko musi miec od 2 do 50 znakow")
            .When(x => x.LastName is not null);

        RuleFor(x => x.RacingNumber)
            .MaximumLength(10).WithMessage("Numer startowy moze miec maksymalnie 10 znakow")
            .When(x => x.RacingNumber is not null);
    }
}
