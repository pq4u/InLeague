using FluentValidation;

namespace InLeague.Application.Features.Karts.Validators;

public class UpdateKartDtoValidator : AbstractValidator<UpdateKartDto>
{
    public UpdateKartDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Numer gokarta nie moze byc pusty")
            .MaximumLength(10).WithMessage("Numer gokarta moze miec maksymalnie 10 znakow")
            .When(x => x.Number is not null);
    }
}
