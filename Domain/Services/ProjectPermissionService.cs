using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Services;

public class ProjectPermissionService : IProjectPermissionService
{
    public bool CanChangeMemberRole(Project project, Guid? initiatorId, Guid? targetMemberId, ProjectRole role)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        var targetMember = project.Members
            .FirstOrDefault(m => m.Id == targetMemberId);

        if (targetMember == null)
            return false;

        if (initiatorRole == ProjectRole.Owner)
            return role != ProjectRole.Owner;

        if (initiatorRole == ProjectRole.Admin)
            return targetMember.Role == ProjectRole.Member ||
                   targetMember.Role == ProjectRole.Viewer;

        return false;
    }

    public bool CanCreateTask(Project project, Guid? initiatorId)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;
            
        return initiatorRole == ProjectRole.Member ||
            initiatorRole == ProjectRole.Admin ||
            initiatorRole == ProjectRole.Owner;
    }

    public bool CanDeleteTask(Project project, Guid? initiatorId, Models.Task task)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;

        if (initiatorRole == ProjectRole.Owner)
            return true;

        if (initiatorRole == ProjectRole.Admin)
            return true;

        if (initiatorRole == ProjectRole.Member)
        {
            var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
            if (member == null)
                return false;

            return task.CreatorMemberId == member.Id;
        }

        return false;
    }

    public bool CanRemoveMember(Project project, Guid? initiatorId, Guid? targetMemberId)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        var targetMember = project.Members
            .FirstOrDefault(m => m.Id == targetMemberId);

        if (targetMember == null)
            return false;

        if (targetMember.Role == ProjectRole.Owner)
            return false;

        if (initiatorRole == ProjectRole.Owner)
            return true;

        if (initiatorRole == ProjectRole.Admin)
            return targetMember.Role == ProjectRole.Member ||
                   targetMember.Role == ProjectRole.Viewer;

        return false;
    }

    public ProjectRole? GetUserRole(Project project, Guid? userId)
    {
        return project.Members
            .FirstOrDefault(m => m.UserId == userId)?.Role;
    }

    public bool CanAddComment(Project project, Guid? initiatorId)
    {
        // Anyone who is a member (even a Viewer) can usually comment, 
        // but we can restrict it if needed. For now, any member.
        return GetUserRole(project, initiatorId) != null;
    }

    public bool CanUpdateComment(Project project, Guid? initiatorId, Comment comment)
    {
        // Only the author can update their comment.
        var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
        return member != null && comment.MemberId == member.Id;
    }

    public bool CanDeleteComment(Project project, Guid? initiatorId, Comment comment)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null) return false;

        // Owner or Admin can delete any comment.
        if (initiatorRole == ProjectRole.Owner || initiatorRole == ProjectRole.Admin)
            return true;

        // Author can delete their own comment.
        var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
        return member != null && comment.MemberId == member.Id;
    }
}