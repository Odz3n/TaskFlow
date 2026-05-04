using FluentValidation;
using TaskFlow.Application.Features.Attachments.Commands;

namespace TaskFlow.Application.Features.Attachments;

public class CreateAttachmentCommandValidator : AbstractValidator<CreateAttachmentCommand>
{
    public CreateAttachmentCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Initiator ID is required");

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required")
            .Must(x => x.Length > 0).WithMessage("File cannot be empty");
    }
}
