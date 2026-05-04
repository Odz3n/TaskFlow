using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Interfaces.Services;

public interface IFileService
{
    Task<string?> SaveFileAsync(
        ContentType contentType,
        Guid? targetId,
        SubContentType subContentType,
        IFormFile file,
        CancellationToken ct);

    Task<bool> DeleteFile(string relativePath, CancellationToken ct);

    Task<bool> DeleteEntityDirectoryAsync(
        ContentType contentType,
        Guid? entityId,
        CancellationToken ct);
}