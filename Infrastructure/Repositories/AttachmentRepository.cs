using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly AppDbContext _db;
    public AttachmentRepository(
        AppDbContext db
    )
    {
        _db = db;
    }
    public async System.Threading.Tasks.Task AddAttachmentAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        await _db.Attachments.AddAsync(attachment, cancellationToken);
    }

    public async Task<bool> AttachmentExistsAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await _db.Attachments
            .AnyAsync(a => a.Id == id, cancellationToken);
    }

    public IQueryable<Attachment> GetAttachmentsQueryable(CancellationToken cancellationToken)
    {
        return GetAttachmentsWithIncludes(asNoTracking: true);
    }

    public async Task<IEnumerable<Attachment>> GetTaskAttachmentsAsync(Guid? taskId, CancellationToken cancellationToken)
    {
        return await GetAttachmentsWithIncludes(asNoTracking: true)
            .Where(a => a.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Attachment?> GetTrackedAttachmentByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await GetAttachmentsWithIncludes(asNoTracking: false)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Attachment?> GetUntrackedAttachmentByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        return await GetAttachmentsWithIncludes(asNoTracking: true)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Attachment>> GetUserAttachmentsAsync(Guid? userId, CancellationToken cancellationToken)
    {
        return await GetAttachmentsWithIncludes(asNoTracking: false)
            .Where(a => a.Member.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveAttachmentAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        _db.Attachments.Remove(attachment);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateAttachmentAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        _db.Attachments.Update(attachment);
    }
    private IQueryable<Attachment> GetAttachmentsWithIncludes(
        bool asNoTracking = false
    )
    {
        var query = _db.Attachments
            .Include(a => a.Task)
            .Include(a => a.Member)
                .ThenInclude(m => m != null ? m.User : null!);

        return asNoTracking ? query.AsNoTracking() : query;        
    }
}