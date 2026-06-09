using FluentValidation;
using InLeague.Application.Features.Leagues.DTOs;

namespace InLeague.Application.Features.Leagues.Validators;

public class CreateLeagueDtoValidator : AbstractValidator<CreateLeagueDto>
{
    public CreateLeagueDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana")
            .Length(3, 100).WithMessage("Nazwa musi miec od 3 do 100 znakow");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis moze miec maksymalnie 500 znakow")
            .When(x => x.Description is not null);

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Data rozpoczecia jest wymagana");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("Data zakonczenia musi byc po dacie rozpoczecia")
            .When(x => x.EndDate.HasValue);
    }
}
