using Mapster;
using TaskFlow.Application.DTOs.Task;
using DomainTask = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Application.Mappings.Mapster;

public class TaskMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Task -> TaskDto
        config.NewConfig<DomainTask, TaskDto>()
            .Map(dest => dest.id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.DueDate, src => src.DueDate);
    }
}
