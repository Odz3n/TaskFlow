using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs.Auth;

public class RoleDto
{
    [Required]
    public string RoleName { get; set; } = null!;
}

public class UserRoleDto
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;
}