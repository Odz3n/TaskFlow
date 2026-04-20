using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs.Auth;

public class RegisterUserRequest
{
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}