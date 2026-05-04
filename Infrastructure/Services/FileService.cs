using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Interfaces.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadsRoot;
    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        _uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }

    public async Task<string?> SaveFileAsync(
        ContentType contentType,
        Guid? targetId,
        SubContentType subContentType,
        IFormFile file,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File was not provided.");

        var contentTypeName = contentType.ToString().ToLowerInvariant();
        var subContentTypeName = subContentType.ToString().ToLowerInvariant();

        // Create folder structure: uploads/{contentType}/{id}/{subContentType}/
        var contentFolder = Path.Combine(_uploadsRoot, contentTypeName, $"{targetId}");
        var targetFolder = Path.Combine(contentFolder, subContentTypeName);

        Directory.CreateDirectory(targetFolder);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{subContentTypeName}_{timestamp}_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(targetFolder, fileName);

        await using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream, ct);
        }

        return Path.Combine("uploads", contentTypeName, $"{targetId}", subContentTypeName, fileName)
            .Replace('\\', '/');
    }
    public async Task<bool> DeleteFile(string relativePath, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(relativePath))
            return false;

        var normalizedRelativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace("uploads\\", "");
        Console.WriteLine(normalizedRelativePath);

        var fullPath = Path.Combine(_uploadsRoot, normalizedRelativePath);
        Console.WriteLine(fullPath);


        if (!File.Exists(fullPath))
            return false;

        await Task.Run(() => File.Delete(fullPath), ct);
        return true;
    }
    public async Task<bool> DeleteEntityDirectoryAsync(
        ContentType contentType,
        Guid? entityId,
        CancellationToken ct)
    {
        var contentTypeName = contentType.ToString().ToLowerInvariant();
        var entityFolder = Path.Combine(_uploadsRoot, contentTypeName, $"{entityId}");
        if (!Directory.Exists(entityFolder))
            return false;

        await Task.Run(() => Directory.Delete(entityFolder, true), ct);
        return true;
    }
}