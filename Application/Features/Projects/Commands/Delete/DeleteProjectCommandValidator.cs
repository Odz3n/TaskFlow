using FluentValidation;

namespace TaskFlow.Application.Features.Projects.Commands;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Project Id is required.");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Initiator Id is required.");
    }
}
