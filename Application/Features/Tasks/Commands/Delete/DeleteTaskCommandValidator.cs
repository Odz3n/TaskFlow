using FluentValidation;
using TaskFlow.Application.Features.Tasks.Commands;

namespace TaskFlow.Application.Features.Tasks;

public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Initiator ID is required");
    }
}
