using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _rootPath = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        var safeFolder = Path.GetFileName(folderName);

        if (string.IsNullOrWhiteSpace(safeFolder))
            throw new ArgumentException("Invalid folder name.");

        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";

        var folderPath = Path.Combine(_rootPath, safeFolder);
        Directory.CreateDirectory(folderPath);

        var finalPath = Path.Combine(folderPath, storedFileName);

        await using var output = new FileStream(
            finalPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await fileStream.CopyToAsync(output, cancellationToken);

        return storedFileName;
    }

    public Task DeleteAsync(
        string fileName,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        var safeFolder = Path.GetFileName(folderName);
        var safeFileName = Path.GetFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeFolder) ||
            string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new ArgumentException("Invalid file path.");
        }

        var path = Path.Combine(_rootPath, safeFolder, safeFileName);

        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }
}