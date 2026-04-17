using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;
    public ProjectRepository(AppDbContext db)
    {
        _db = db;
    }

    public async System.Threading.Tasks.Task AddProjectAsync(Project project, CancellationToken cancellationToken)
    {
        await _db.Projects.AddAsync(project, cancellationToken);
    }

    public async Task<List<Project>> GetOwnedProjectsAsync(Guid? ownerId, CancellationToken cancellationToken)
    {
        if (ownerId == null)
            return new List<Project>();

        return await GetProjectWithIncludes(true)
            .Where(p => p.Members.Any(m => m.UserId == ownerId && m.Role == ProjectRole.Owner))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetProjectMembersCountAsync(Guid? projectId, CancellationToken cancellationToken)
    {
        if (projectId == null)
            return 0;

        return await _db.Members
            .CountAsync(m => m.ProjectId == projectId && m.IsActive, cancellationToken);
    }

    public IQueryable<Project> GetProjectsQueryable(CancellationToken cancellationToken)
    {
        return _db.Projects.AsQueryable();
    }

    public async Task<Project?> GetTrackedProjectByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return null;

        return await GetProjectWithIncludes()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetUntrackedProjectByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return null;

        return await GetProjectWithIncludes(true)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Project>> GetUserProjectsAsync(Guid? userId, CancellationToken cancellationToken)
    {
        if (userId == null)
            return new List<Project>();

        return await _db.Projects
            .Where(p => p.Members.Any(m => m.UserId == userId && m.IsActive))
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .Include(p => p.Tasks)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserMemberOfProjectAsync(Guid? projectId, Guid? userId, CancellationToken cancellationToken)
    {
        if (projectId == null || userId == null)
            return false;

        return await _db.Projects
            .AnyAsync(p => p.Id == projectId && p.Members.Any(m => m.UserId == userId && m.IsActive),
                cancellationToken);
    }

    public async Task<bool> ProjectExistsAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return false;

        return await _db.Projects
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveProjectAsync(Project project, CancellationToken cancellationToken)
    {
        _db.Remove(project);
    }

    public async System.Threading.Tasks.Task UpdateProjectAsync(Project project, CancellationToken cancellationToken)
    {
        _db.Update(project);
    }

    public async System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
    private IQueryable<Project> GetProjectWithIncludes(bool asNoTracking = false)
    {
        var query = _db.Projects
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .Include(p => p.Members)
                .ThenInclude(m => m.AssignedTasks)
            .Include(p => p.Members)
                .ThenInclude(m => m.CreatedTasks)
            .Include(p => p.Members)
                .ThenInclude(m => m.Comments)
            .Include(p => p.Members)
                .ThenInclude(m => m.Attachments)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.AssigneeMember)
                    .ThenInclude(am => am.User)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.CreatorMember)
                    .ThenInclude(cm => cm.User)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Comments)
                    .ThenInclude(c => c.Member)
                        .ThenInclude(m => m.User)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Attachments)
                    .ThenInclude(a => a.Member)
                        .ThenInclude(um => um.User);

        return asNoTracking ? query.AsNoTracking() : query;
    }
}