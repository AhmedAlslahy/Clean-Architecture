using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services;

public sealed class FilePathService(
    IWebHostEnvironment environment)
    : IFilePathService
{
    public string GetPath(string relativePath)
    {
        return Path.Combine(
            environment.WebRootPath,
            relativePath);
    }
}