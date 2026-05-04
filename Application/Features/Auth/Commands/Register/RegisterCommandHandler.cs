using Mapster;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IFileService _fileService;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        IFileService fileService)
    {
        _userManager = userManager;
        _fileService = fileService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result<RegisterResponse>.Failure(DomainErrors.User.AlreadyExists);



        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName ?? request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = new Error(
                Code: "Create error",
                Description: "Error on CreateAsync operation"
            );

            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description))
                .ToList();

            return Result<RegisterResponse>.Failure(error, errors);
        }

        var fileServiceResult = await _fileService.SaveFileAsync(
            ContentType.Users,
            user.Id,
            SubContentType.Avatars,
            request.Avatar,
            cancellationToken
        );

        user.AvatarUrl = fileServiceResult;

        await _userManager.AddToRoleAsync(user, "User");

        var response = user.Adapt<RegisterResponse>();

        return Result<RegisterResponse>.Success(response);
    }
}