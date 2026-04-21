

// using TaskFlow.Application.Features.Attachments.Commands;

namespace TaskFlow.Application.Features.ValidationsTODO;

/*
public class UploadAttachmentCommandValidator: AbstractValidator<UploadAttachmentCommand>
{
    public UploadAttachmentCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name is too long");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Attachment must be linked to a task");

        RuleFor(x => x.FileSize)
            .ExclusiveBetween(0, 10 * 1024 * 1024)
            .WithMessage("File size must be less than 10MB");
    }
}
*/