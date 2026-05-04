namespace TaskFlow.Domain.Models;

public class Attachment
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }
    public Task Task { get; set; } = null!;

    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long FileSize { get; set; }

    public Guid? MemberId { get; set; }
    public Member? Member { get; set; }
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}