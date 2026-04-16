using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Domain.Models;

public class User : IdentityUser<Guid>
{
    private const string DefaultAvatarUrl = "TO BE ADDED";
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? AvatarUrl { get; set; }

    public string EffectiveAvatarUrl => AvatarUrl ?? DefaultAvatarUrl;

    public ICollection<Member> Memberships { get; set; } = new List<Member>();
}