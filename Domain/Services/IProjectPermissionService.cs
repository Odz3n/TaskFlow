using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Services;

public interface IProjectPermissionService
{
    ProjectRole? GetUserRole(Project project, Guid? userId);

    bool CanCreateTask(Project project, Guid? initiatorId);
    bool CanDeleteTask(Project project, Guid? initiatorId, Models.Task task);
    bool CanUpdateTask(Project project, Guid? initiatorId, Models.Task task);

    bool CanCreateAttachment(Project project, Guid? initiatorId);
    bool CanUpdateAttachment(Project project, Guid? initiatorId, Attachment attachment);
    bool CanDeleteAttachment(Project project, Guid? initiatorId, Attachment attachment);

    bool CanRemoveMember(Project project, Guid? initiatorId, Guid? targetMemberId);
    bool CanChangeMemberRole(Project project, Guid? initiatorId, Guid? targetMemberId, ProjectRole role);
    
    bool CanAddComment(Project project, Guid? initiatorId);
    bool CanUpdateComment(Project project, Guid? initiatorId, Comment comment);
    bool CanDeleteComment(Project project, Guid? initiatorId, Comment comment);
}