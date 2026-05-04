using System.Linq.Expressions;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Features.Comments.Queries;

public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, PagedResult<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentsQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Result<PagedResult<CommentDto>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var sortingMap = new Dictionary<string, Expression<Func<Comment, object>>>
        {
            ["createddate"] = c => c.CreatedDate
        };

        var query = _commentRepository.GetCommentsQueryable(cancellationToken)
            .Where(c => c.TaskId == request.TaskId);

        if (!string.IsNullOrWhiteSpace(request.Parameters.Search))
        {
            var searchTerm = request.Parameters.Search.ToLower();
            query = query.Where(c => c.Text.ToLower().Contains(searchTerm));
        }

        var result = await query
            .ApplySort(request.Parameters, sortingMap)
            .ToPagedResultAsync<Comment, CommentDto>(request.Parameters, cancellationToken);

        return Result<PagedResult<CommentDto>>.Success(result);
    }
}
