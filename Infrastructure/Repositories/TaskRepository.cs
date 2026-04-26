using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;
    public TaskRepository(
        AppDbContext db
    )
    {
        _db = db;
    }
    public async System.Threading.Tasks.Task AddTaskAsync(Domain.Models.Task task, CancellationToken cancellationToken)
    {
        await _db.Tasks.AddAsync(task, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Models.Task>> GetProjectTasksAsync(Guid? projectId, CancellationToken cancellationToken)
    {
        return await _db.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Models.Task>> GetTasksByAssigneeAsync(Guid? assigneeId, CancellationToken cancellationToken)
    {
        return await _db.Tasks
            .Where(t => t.AssigneeMemberId == assigneeId)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<Domain.Models.Task> GetTasksQueryable(CancellationToken cancellationToken)
    {
        return _db.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.AssigneeMember)
                .ThenInclude(am => am != null ? am.User : null!)
            .Include(t => t.CreatorMember)
                .ThenInclude(cm => cm != null ? cm.User : null!)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .AsQueryable();
    }

    public async Task<Domain.Models.Task?> GetTrackedTaskByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return null;

        return await GetTasksWithIncludes(asNoTracking: false)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Domain.Models.Task?> GetUntrackedTaskByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return null;

        return await GetTasksWithIncludes(asNoTracking: true)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveTaskAsync(Domain.Models.Task task, CancellationToken cancellationToken)
    {
        _db.Remove(task);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> TaskExistsAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return false;

        return await _db.Tasks
            .AnyAsync(t => t.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(Domain.Models.Task task, CancellationToken cancellationToken)
    {
        _db.Tasks.Update(task);
    }
    private IQueryable<Domain.Models.Task> GetTasksWithIncludes(
        bool asNoTracking = false,
        bool includeComments = false,
        bool includeAttachments = false)
    {
        var query = _db.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssigneeMember)
                .ThenInclude(am => am != null ? am.User : null!)
            .Include(t => t.CreatorMember)
                .ThenInclude(cm => cm != null ? cm.User : null!)
            .AsQueryable();

        if (includeComments)
            query = query.Include(t => t.Comments)
                .ThenInclude(c => c.Member)
                    .ThenInclude(m => m != null ? m.User : null!);

        if (includeAttachments)
            query = query.Include(t => t.Attachments)
                .ThenInclude(a => a.Member)
                    .ThenInclude(m => m != null ? m.User : null!);

        return asNoTracking ? query.AsNoTracking() : query;
    }
}