using Mapster;
using TaskFlow.Domain.Models;
using TaskFlow.Application.DTOs.Attachment;

namespace TaskFlow.Application.Mappings.Mapster;

public class AttachmentMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Attachment -> AttachmentDto
        config.NewConfig<Attachment, AttachmentDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.TaskId, src => src.TaskId)
            .Map(dest => dest.MemberId, src => src.MemberId)
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.FilePath, src => src.FilePath)
            .Map(dest => dest.FileSize, src => src.FileSize)
            .Map(dest => dest.UploadedDate, src => src.UploadedDate);
    }
}
