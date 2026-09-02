using FluentValidation;
using JobFlow.Application.Jobs.DTOs;

namespace JobFlow.Application.Jobs.Validators;

public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Payload)
            .NotEmpty()
            .MaximumLength(10000);
    }
}
