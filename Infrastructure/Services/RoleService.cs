using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Models;


namespace TaskFlow.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleService(RoleManager<IdentityRole> roleManager,
                       UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateRoleAsync(RoleDto role)
    {
        if (await _roleManager.RoleExistsAsync(role.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Role '{role.RoleName}' already exist"
            });

        return await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Role '{roleName}' not found"
            });

        return await _roleManager.DeleteAsync(role);
    }

    public List<string> GetAllRoles()
    {
        return _roleManager.Roles.Select(r => r.Name!).ToList();
    }

    public async Task<IdentityResult> AssignRoleAsync(UserRoleDto userRole)
    {
        var user = await _userManager.FindByIdAsync(userRole.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "User not found"
            });

        if (!await _roleManager.RoleExistsAsync(userRole.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Role '{userRole.RoleName}' does not exist"
            });

        return await _userManager.AddToRoleAsync(user, userRole.RoleName);
    }

    public async Task<IdentityResult> RemoveRoleAsync(UserRoleDto userRole)
    {
        var user = await _userManager.FindByIdAsync(userRole.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "User not found"
            });

        return await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }
}