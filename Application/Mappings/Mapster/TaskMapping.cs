using System.Net.Mail;
using Mapster;
using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.DTOs.Task;
using DomainTask = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Application.Mappings.Mapster;

public class TaskMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Task -> TaskDto
        config.NewConfig<DomainTask, TaskDto>();

        // Task -> TaskDetailDto
        config.NewConfig<DomainTask, TaskDetailDto>()
            .Map(dest => dest.Comments, src => src.Comments.Adapt<List<CommentDto>>())
            .Map(dest => dest.Attachments, src => src.Attachments.Adapt<List<AttachmentDto>>());
    }
}
