
using TaskFlow.Application.Interfaces.Services;

namespace TaskFlow.Infrastructure.Services;

public class ProjectAuthorizationService : IProjectAuthorizationService
{
    public Task<bool> CanAssignTaskAsync(Guid userId, Guid taskId, Guid assigneeId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanDeleteProjectAsync(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanEditProjectAsync(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanEditTaskAsync(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanManageMembersAsync(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanViewProjectAsync(Guid userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanViewTaskAsync(Guid userId, Guid taskId)
    {
        throw new NotImplementedException();
    }
}