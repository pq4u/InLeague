using FluentValidation;
using InLeague.Application.Features.Karts.DTOs;

namespace InLeague.Application.Features.Karts.Validators;

public class CreateKartDtoValidator : AbstractValidator<CreateKartDto>
{
    public CreateKartDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Numer gokarta jest wymagany")
            .MaximumLength(10).WithMessage("Numer gokarta moze miec maksymalnie 10 znakow");
    }
}
