using Mapster;
using TaskFlow.Domain.Models;
using TaskFlow.Application.DTOs.Comment;

namespace TaskFlow.Application.Mappings.Mapster;

public class CommentMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Comment -> CommentDto
        config.NewConfig<Comment, CommentDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TaskId, src => src.TaskId)
            .Map(dest => dest.MemberId, src => src.MemberId)
            .Map(dest => dest.Text, src => src.Text)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate)
            .Map(dest => dest.UpdatedDate, src => src.UpdatedDate)
            .Map(dest => dest.AttachmentFilePath, src => src.AttachmentFilePath);
    }
}
