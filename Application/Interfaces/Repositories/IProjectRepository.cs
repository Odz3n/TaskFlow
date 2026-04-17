using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    System.Threading.Tasks.Task AddProjectAsync(Project project, CancellationToken cancellationToken);

    IQueryable<Project> GetProjectsQueryable(CancellationToken cancellationToken);
    System.Threading.Tasks.Task<Project?> GetTrackedProjectByIdAsync(Guid? id, CancellationToken cancellationToken);
    System.Threading.Tasks.Task<Project?> GetUntrackedProjectByIdAsync(Guid? id, CancellationToken cancellationToken);
    Task<List<Project>> GetUserProjectsAsync(Guid? userId, CancellationToken cancellationToken);
    Task<List<Project>> GetOwnedProjectsAsync(Guid? ownerId, CancellationToken cancellationToken);

    System.Threading.Tasks.Task UpdateProjectAsync(Project project, CancellationToken cancellationToken);

    System.Threading.Tasks.Task RemoveProjectAsync(Project project, CancellationToken cancellationToken);

    Task<bool> ProjectExistsAsync(Guid? id, CancellationToken cancellationToken);
    Task<bool> IsUserMemberOfProjectAsync(Guid? projectId, Guid? userId, CancellationToken cancellationToken);
    Task<int> GetProjectMembersCountAsync(Guid? projectId, CancellationToken cancellationToken);

    System.Threading.Tasks.Task SaveChangesAsync(CancellationToken cancellationToken);
}