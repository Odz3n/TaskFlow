using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Models;
public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? AssigneeMemberId { get; set; }
    public Member? AssigneeMember { get; set; }
    
    public Guid? CreatorMemberId { get; set; }
    public Member? CreatorMember { get; set; }

    public Status Status { get; set; }
    public Priority Priority { get; set; }
    
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}