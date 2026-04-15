namespace TaskFlow.Domain.Models;

public class Comment
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }
    public Task Task { get; set; } = null!;

    public Guid? MemberId { get; set; }
    public Member? Member { get; set; }

    public string Text { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? AttachmentFilePath { get; set; }
}