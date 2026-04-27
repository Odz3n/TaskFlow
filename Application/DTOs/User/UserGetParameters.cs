using TaskFlow.Application.Common;

namespace TaskFlow.Application.DTOs.User;

public class UserGetParameters: QueryParameters
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
}