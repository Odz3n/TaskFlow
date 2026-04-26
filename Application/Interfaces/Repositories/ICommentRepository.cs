using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    System.Threading.Tasks.Task AddCommentAsync(Comment comment, CancellationToken cancellationToken);

    IQueryable<Comment> GetCommentsQueryable(CancellationToken cancellationToken);
    Task<Comment?> GetTrackedCommentByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<Comment?> GetUntrackedCommentByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid? taskId, CancellationToken cancellationToken);
    Task<IEnumerable<Comment>> GetUserCommentsAsync(Guid? userId, CancellationToken cancellationToken);
    Task<IEnumerable<Comment>> GetMemberCommentsAsync(Guid? memberId, CancellationToken cancellationToken);

    Task<bool> CommentExistsAsync(Guid? id, CancellationToken cancellationToken);

    System.Threading.Tasks.Task UpdateCommentAsync(Comment comment, CancellationToken cancellationToken);

    System.Threading.Tasks.Task RemoveCommentAsync(Comment comment, CancellationToken cancellationToken);

    System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken);
}