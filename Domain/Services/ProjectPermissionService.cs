using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Services;

/// <summary>
/// Service for checking user permissions within a project.
/// </summary>
public class ProjectPermissionService : IProjectPermissionService
{
    /// <summary>
    /// Checks if the initiator can change a project member's role.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="targetMemberId">The unique identifier of the member whose role is being changed.</param>
    /// <param name="role">The new role to assign.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
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

    /// <summary>
    /// Checks if the initiator can create an attachment in the project.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanCreateAttachment(Project project, Guid? initiatorId)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;

        return initiatorRole == ProjectRole.Owner ||
               initiatorRole == ProjectRole.Admin ||
               initiatorRole == ProjectRole.Member;
    }

    /// <summary>
    /// Checks if the initiator can create a task in the project.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanCreateTask(Project project, Guid? initiatorId)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;

        return initiatorRole == ProjectRole.Member ||
            initiatorRole == ProjectRole.Admin ||
            initiatorRole == ProjectRole.Owner;
    }

    /// <summary>
    /// Checks if the initiator can delete a task.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="task">The task to delete.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
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

    /// <summary>
    /// Checks if the initiator can remove a member from the project.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="targetMemberId">The unique identifier of the member to remove.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
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

    /// <summary>
    /// Gets the role of a user within a project.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The user's project role, or null if they are not a member.</returns>
    public ProjectRole? GetUserRole(Project project, Guid? userId)
    {
        return project.Members
            .FirstOrDefault(m => m.UserId == userId)?.Role;
    }

    /// <summary>
    /// Checks if the initiator can add a comment to any task in the project.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanAddComment(Project project, Guid? initiatorId)
    {

        return GetUserRole(project, initiatorId) != null;
    }

    /// <summary>
    /// Checks if the initiator can update a specific comment.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="comment">The comment to update.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanUpdateComment(Project project, Guid? initiatorId, Comment comment)
    {
        var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
        return member != null && comment.MemberId == member.Id;
    }

    /// <summary>
    /// Checks if the initiator can delete a specific comment.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="comment">The comment to delete.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanDeleteComment(Project project, Guid? initiatorId, Comment comment)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null) return false;

        if (initiatorRole == ProjectRole.Owner || initiatorRole == ProjectRole.Admin)
            return true;

        var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
        return member != null && comment.MemberId == member.Id;
    }

    /// <summary>
    /// Checks if the initiator can update a task.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="task">The task to update.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanUpdateTask(Project project, Guid? initiatorId, Models.Task task)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;

        if (initiatorRole == ProjectRole.Owner ||
            initiatorRole == ProjectRole.Admin)
            return true;

        if (initiatorRole == ProjectRole.Member)
        {
            var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
            return member != null && task.CreatorMemberId == member.Id;
        }

        return false;
    }

    /// <summary>
    /// Checks if the initiator can update an attachment.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="attachment">The attachment to update.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanUpdateAttachment(Project project, Guid? initiatorId, Attachment attachment)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;

        if (initiatorRole == ProjectRole.Owner ||
            initiatorRole == ProjectRole.Admin ||
            initiatorRole == ProjectRole.Member)
        {
            var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
            return member != null && attachment.MemberId == member.Id;
        }
        return false;
    }

    /// <summary>
    /// Checks if the initiator can delete an attachment.
    /// </summary>
    /// <param name="project">The project context.</param>
    /// <param name="initiatorId">The unique identifier of the user initiating the action.</param>
    /// <param name="attachment">The attachment to delete.</param>
    /// <returns>True if allowed; otherwise, false.</returns>
    public bool CanDeleteAttachment(Project project, Guid? initiatorId, Attachment attachment)
    {
        var initiatorRole = GetUserRole(project, initiatorId);
        if (initiatorRole == null)
            return false;

        if (initiatorRole == ProjectRole.Owner ||
            initiatorRole == ProjectRole.Admin)
            return true;

        if (initiatorRole == ProjectRole.Member)
        {
            var member = project.Members.FirstOrDefault(m => m.UserId == initiatorId);
            return member != null && attachment.MemberId == member.Id;
        }
        return false;
    }
}