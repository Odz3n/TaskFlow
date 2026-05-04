using TaskFlow.Application.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Task;

public class TaskGetParameters
    : QueryParameters
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? AssigneeMemberId { get; set; }
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
}
