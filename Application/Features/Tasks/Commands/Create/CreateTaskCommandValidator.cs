using FluentValidation;
using TaskFlow.Application.Features.Tasks.Commands;

namespace TaskFlow.Application.Features.Tasks;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.AssigneeMemberId)
            .NotEmpty().WithMessage("Assignee ID is required");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority level");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future");
    }
}
