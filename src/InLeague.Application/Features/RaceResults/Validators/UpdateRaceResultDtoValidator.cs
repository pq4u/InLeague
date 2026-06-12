using FluentValidation;

namespace InLeague.Application.Features.RaceResults.Validators;

public class UpdateRaceResultDtoValidator : AbstractValidator<UpdateRaceResultDto>
{
    public UpdateRaceResultDtoValidator()
    {
        RuleFor(x => x.LapTimeMs)
            .GreaterThan(0).WithMessage("Czas okrazenia musi byc wiekszy od 0")
            .When(x => x.LapTimeMs.HasValue);

        RuleFor(x => x.TotalTimeMs)
            .GreaterThan(0).WithMessage("Laczny czas musi byc wiekszy od 0")
            .When(x => x.TotalTimeMs.HasValue);

        RuleFor(x => x.StartingPosition)
            .GreaterThan(0).WithMessage("Pozycja startowa musi byc wieksza od 0")
            .When(x => x.StartingPosition.HasValue);

        RuleFor(x => x.FinishingPosition)
            .GreaterThan(0).WithMessage("Pozycja koncowa musi byc wieksza od 0")
            .When(x => x.FinishingPosition.HasValue);

        RuleFor(x => x.LapsCompleted)
            .GreaterThanOrEqualTo(0).WithMessage("Liczba ukonczonych okrazen nie moze byc ujemna")
            .When(x => x.LapsCompleted.HasValue);
    }
}
