using FluentValidation;

namespace TaskFlow.Application.Features.Comments.Commands;

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("Comment Id is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required.");
    }
}
