using FluentValidation;

namespace InLeague.Application.Features.Leagues.Validators;

public class UpdateLeagueDtoValidator : AbstractValidator<UpdateLeagueDto>
{
    public UpdateLeagueDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa nie moze byc pusta")
            .Length(3, 100).WithMessage("Nazwa musi miec od 3 do 100 znakow")
            .When(x => x.Name is not null);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate.Value).WithMessage("Data zakonczenia musi byc po dacie rozpoczecia")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
