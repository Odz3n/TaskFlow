namespace TaskFlow.Application.Features.ValidationsTODO;

/*public class CreateTaskCommandValidator: AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description is too long")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Task must be assigned to a valid project");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid task status");
        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority level");
        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("The deadline must be a future date");
        RuleFor(x => x.AssigneeMemberId)
            .NotEqual(Guid.Empty).WithMessage("Assignee ID cannot be an empty GUID")
            .When(x => x.AssigneeMemberId.HasValue);
    }
}*/
