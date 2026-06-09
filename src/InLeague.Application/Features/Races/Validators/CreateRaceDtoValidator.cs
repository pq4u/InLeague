using FluentValidation;
using InLeague.Application.Features.Races.DTOs;

namespace InLeague.Application.Features.Races.Validators;

public class CreateRaceDtoValidator : AbstractValidator<CreateRaceDto>
{
    public CreateRaceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana")
            .Length(3, 100).WithMessage("Nazwa musi miec od 3 do 100 znakow");

        RuleFor(x => x.RaceDate)
            .NotEmpty().WithMessage("Data wyscigu jest wymagana");

        RuleFor(x => x.NumberOfLaps)
            .GreaterThan(0).WithMessage("Liczba okrazen musi byc wieksza od 0");
    }
}
