using FluentValidation;
using InLeague.Application.Features.Races.DTOs;

namespace InLeague.Application.Features.Races.Validators;

public class UpdateRaceDtoValidator : AbstractValidator<UpdateRaceDto>
{
    public UpdateRaceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa nie moze byc pusta")
            .Length(3, 100).WithMessage("Nazwa musi miec od 3 do 100 znakow")
            .When(x => x.Name is not null);

        RuleFor(x => x.NumberOfLaps)
            .GreaterThan(0).WithMessage("Liczba okrazen musi byc wieksza od 0")
            .When(x => x.NumberOfLaps.HasValue);
    }
}
