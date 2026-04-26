using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces.Repositories;

public interface IAttachmentRepository
{
    System.Threading.Tasks.Task AddAttachmentAsync(Attachment attachment, CancellationToken cancellationToken);

    IQueryable<Attachment> GetAttachmentsQueryable(CancellationToken cancellationToken);
    Task<Attachment?> GetTrackedAttachmentByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<Attachment?> GetUntrackedAttachmentByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<IEnumerable<Attachment>> GetTaskAttachmentsAsync(Guid? taskId, CancellationToken cancellationToken);
    Task<IEnumerable<Attachment>> GetUserAttachmentsAsync(Guid? userId, CancellationToken cancellationToken);

    Task<bool> AttachmentExistsAsync(Guid? id, CancellationToken cancellationToken);

    System.Threading.Tasks.Task UpdateAttachmentAsync(Attachment attachment, CancellationToken cancellationToken);

    System.Threading.Tasks.Task RemoveAttachmentAsync(Attachment attachment, CancellationToken cancellationToken);

    System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken);
}