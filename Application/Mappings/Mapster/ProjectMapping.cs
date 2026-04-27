using Mapster;
using TaskFlow.Domain.Models;
using TaskFlow.Application.DTOs.Project;

namespace TaskFlow.Application.Mappings.Mapster;

public class ProjectMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Project -> ProjectDto
        config.NewConfig<Project, ProjectDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate)
            .Map(dest => dest.IsArchived, src => src.IsArchived);
    }
}
