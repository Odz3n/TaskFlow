using FluentValidation;

namespace TaskFlow.Application.Features.Comments.Commands;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task Id is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text is required.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }
}
