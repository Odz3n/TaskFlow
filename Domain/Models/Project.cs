namespace TaskFlow.Domain.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; }
    
    public ICollection<Member> Members { get; set; } = new List<Member>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}