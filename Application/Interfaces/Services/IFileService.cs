using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Interfaces.Services;

/// <summary>
/// Interface for managing file storage operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Saves a file to the specified storage location.
    /// </summary>
    /// <param name="contentType">The top-level category (e.g., Projects, Tasks, Users).</param>
    /// <param name="targetId">The unique identifier of the target entity.</param>
    /// <param name="subContentType">The sub-category (e.g., Avatars, Attachments).</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The relative path to the saved file, or null on failure.</returns>
    Task<string?> SaveFileAsync(
        ContentType contentType,
        Guid? targetId,
        SubContentType subContentType,
        IFormFile file,
        CancellationToken ct);

    /// <summary>
    /// Deletes a specific file from the storage.
    /// </summary>
    /// <param name="relativePath">The relative path of the file to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteFile(string relativePath, CancellationToken ct);

    /// <summary>
    /// Deletes the entire directory associated with an entity.
    /// </summary>
    /// <param name="contentType">The category of the entity.</param>
    /// <param name="entityId">The unique identifier of the entity.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the directory was deleted; otherwise, false.</returns>
    Task<bool> DeleteEntityDirectoryAsync(
        ContentType contentType,
        Guid? entityId,
        CancellationToken ct);
}