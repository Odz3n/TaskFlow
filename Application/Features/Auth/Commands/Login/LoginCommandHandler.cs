using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly UserManager<User> _userManager;
    public LoginCommandHandler(
        JwtTokenService jwtTokenService,
        UserManager<User> userManager
    )
    {
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
    }
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result<LoginResponse>.Failure(new Error("User.NotRegistered", $"User with email '{request.Email}' is not registered"));

        var isPassCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPassCorrect)
            return Result<LoginResponse>.Failure(new Error("User.InvalidPass", "Invalid password"));

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);

        var response = new LoginResponse(
            AccessToken: token,
            TokenType: "Bearer"
        );

        return Result<LoginResponse>.Success(response);
    }
}