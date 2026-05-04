using FluentValidation;

namespace TaskFlow.Application.Features.Comments.Commands;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("Comment Id is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text is required.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
    }
}
