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
}