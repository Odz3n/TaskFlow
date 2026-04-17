namespace TaskFlow.Application.Interfaces.Services;

public interface IProjectAuthorizationService
{
    Task<bool> CanViewProjectAsync(Guid userId, Guid projectId);
    Task<bool> CanEditProjectAsync(Guid userId, Guid projectId);
    Task<bool> CanDeleteProjectAsync(Guid userId, Guid projectId);
    Task<bool> CanManageMembersAsync(Guid userId, Guid projectId);
    Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId);
    Task<bool> CanAssignTaskAsync(Guid userId, Guid taskId, Guid assigneeId);
    Task<bool> CanViewTaskAsync(Guid userId, Guid taskId);
    Task<bool> CanEditTaskAsync(Guid userId, Guid taskId);
}