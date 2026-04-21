using FluentValidation;

namespace TaskFlow.Application.Features.Roles.Commands;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .MinimumLength(3).WithMessage("Role name must be at least 3 characters long")
            .MaximumLength(20).WithMessage("Role name is too long");
    }
}