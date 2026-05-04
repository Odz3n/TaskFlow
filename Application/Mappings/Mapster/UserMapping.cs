using Mapster;
using TaskFlow.Domain.Models;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Application.DTOs.Auth;

namespace TaskFlow.Application.Mappings.Mapster;

public class UserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // User -> UserDto
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.UserName, src => src.UserName ?? string.Empty)
            .Map(dest => dest.AvatarUrl, src => src.EffectiveAvatarUrl);

        // User -> GetUsersResponse
        config.NewConfig<User, GetUsersResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.UserName, src => src.UserName ?? string.Empty)
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(dest => dest.AvatarUrl, src => src.EffectiveAvatarUrl);

        // User -> RegisterResponse
        config.NewConfig<User, RegisterResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.AvatarUrl, src => src.AvatarUrl)
            .Map(dest => dest.Message, src => $"User {src.Email} successfully registered");
    }
}
