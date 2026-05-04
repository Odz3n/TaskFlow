using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Roles.Commands;

public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, CreateRoleResponse>
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    public CreateRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<CreateRoleResponse>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var roleExists = await _roleManager.RoleExistsAsync(request.Name);
        if (roleExists)
            return Result<CreateRoleResponse>.Failure(
                new Error("Roles.AlreadyExists", $"Role '{request.Name}' already exists"));
        
        var role = new IdentityRole<Guid>
        {
            Name = request.Name
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var error = new Error("Roles.CreationFailed", "Failed to create role");
            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description))
                .ToList();

            return Result<CreateRoleResponse>.Failure(error, errors);
        }

        var response = new CreateRoleResponse(Message: $"Role '{request.Name}' created successfully");
        return Result<CreateRoleResponse>.Success(response);
    }
}