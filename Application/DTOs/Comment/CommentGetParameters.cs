using TaskFlow.Application.Common;

namespace TaskFlow.Application.DTOs.Comment;

public class CommentGetParameters : QueryParameters
{
    public string? Search { get; set; }
}
