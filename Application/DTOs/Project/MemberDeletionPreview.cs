using TaskFlow.Application.DTOs.Task;

namespace TaskFlow.Application.DTOs.Project;

public record MemberDeletionPreview(
    int AssignedTasksCount,
    int CreatedTasksCount,
    int CommentsCount,
    int AttachmentsCount,
    IEnumerable<TaskDto> AssignedTasks,
    IEnumerable<TaskDto> CreatedTasks
)
{
    public bool HasTasks => AssignedTasksCount > 0 || CreatedTasksCount > 0;
    public bool HasComments => CommentsCount > 0;
    public bool HasAttachments => AttachmentsCount > 0;
    public static MemberDeletionPreview Empty => new(
        AssignedTasksCount: 0,
        CreatedTasksCount: 0,
        CommentsCount: 0,
        AttachmentsCount: 0,
        AssignedTasks: new List<TaskDto>(),
        CreatedTasks: new List<TaskDto>()
    );
};