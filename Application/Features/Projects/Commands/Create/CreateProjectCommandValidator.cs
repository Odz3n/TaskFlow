using FluentValidation;
using TaskFlow.Application.Features.Projects.Commands;

namespace TaskFlow.Application.Features.Projects.Validations;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Project name is required")
            .MaximumLength(50).WithMessage("Project name must be less than 50 characters")
            .MinimumLength(3).WithMessage("Project name must be greater than 3 characters");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Project description is required")
            .MaximumLength(500).WithMessage("Project description must be less than 500 characters");
        RuleFor(x => x.MemberIds).Must(ids => ids == null || ids.All(id => id != Guid.Empty))
            .WithMessage("Member IDs cannot contain empty GUIDs");
    }
}