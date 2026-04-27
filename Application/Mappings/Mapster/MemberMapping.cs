using Mapster;
using TaskFlow.Domain.Models;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Application.DTOs.Project;

namespace TaskFlow.Application.Mappings.Mapster;

public class MemberMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Member -> MemberDto
        config.NewConfig<Member, MemberDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.User, src => src.User.Adapt<UserDto>())
            .Map(dest => dest.Project, src => src.Project.Adapt<ProjectDto>())
            .Map(dest => dest.Role, src => src.Role)
            .Map(dest => dest.JoinedDate, src => src.JoinedDate)
            .Map(dest => dest.IsActive, src => src.IsActive);
    }
}
