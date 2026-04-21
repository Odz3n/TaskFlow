

// using TaskFlow.Application.Features.Comments.Commands;

namespace TaskFlow.Application.Features.ValidationsTODO;

/*
public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text cannot be empty")
            .MaximumLength(1000).WithMessage("Comment is too long (max 1000 characters)");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Comment must be linked to a task");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Author (Member) is required");
    }
}
*/