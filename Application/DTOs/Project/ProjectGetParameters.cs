using TaskFlow.Application.Common;

namespace TaskFlow.Application.DTOs.Project;

public class ProjectGetParameters : QueryParameters
{
    public string? Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool? IsArchived { get; set; }
}