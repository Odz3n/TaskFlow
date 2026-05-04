using TaskFlow.Application.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Member;

public class MemberGetParameters : QueryParameters
{
    public string? UserName {get; set;} = null!;
    public ProjectRole? ProjectRole { get; set; }
    public DateTime? JoinedDate { get; set; }
    public bool? IsActive { get; set; }
}