using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _db;
    public CommentRepository(
        AppDbContext db
    )
    {
        _db = db;
    }
    public async System.Threading.Tasks.Task AddCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        await _db.Comments.AddAsync(comment, cancellationToken);
    }

    public async Task<bool> CommentExistsAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await _db.Comments
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public IQueryable<Comment> GetCommentsQueryable(CancellationToken cancellationToken)
    {
        return GetCommentsWithIncludes(asNoTracking: true);
    }

    public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid? taskId, CancellationToken cancellationToken)
    {
        return await GetCommentsWithIncludes(asNoTracking: true)
            .Where(c => c.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Comment?> GetTrackedCommentByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await GetCommentsWithIncludes(asNoTracking: false)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Comment?> GetUntrackedCommentByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await GetCommentsWithIncludes(asNoTracking: true)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetUserCommentsAsync(Guid? userId, CancellationToken cancellationToken)
    {
        return await GetCommentsWithIncludes(asNoTracking: true)
            .Where(c => c.Member.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetMemberCommentsAsync(Guid? memberId, CancellationToken cancellationToken)
    {
        return await GetCommentsWithIncludes(asNoTracking: true)
            .Where(c => c.MemberId == memberId)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        _db.Comments.Remove(comment);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateCommentAsync(Comment comment, CancellationToken cancellationToken)
    {
        _db.Comments.Update(comment);
    }

    private IQueryable<Comment> GetCommentsWithIncludes(
        bool asNoTracking = false
    )
    {
        var query = _db.Comments
            .Include(c => c.Task)
            .Include(c => c.Member)
                .ThenInclude(m => m != null ? m.User : null!);

        return asNoTracking ? query.AsNoTracking() : query;
    }
}