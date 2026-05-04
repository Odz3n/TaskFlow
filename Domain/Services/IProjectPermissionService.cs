using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Services;

public interface IProjectPermissionService
{
    ProjectRole? GetUserRole(Project project, Guid? userId);
    bool CanCreateTask(Project project, Guid? initiatorId);
    bool CanDeleteTask(Project project, Guid? initiatorId, Models.Task task);
    bool CanRemoveMember(Project project, Guid? initiatorId, Guid? targetMemberId);
    bool CanChangeMemberRole(Project project, Guid? initiatorId, Guid? targetMemberId, ProjectRole role);
}