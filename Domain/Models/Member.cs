using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Models;

public class Member
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ProjectRole Role { get; set; }
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }

    public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
    public ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}