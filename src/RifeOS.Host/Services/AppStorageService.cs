using System.IO;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class AppStorageService : IAppStorage
{
    private readonly string _baseDir;

    public AppStorageService(string baseDir)
    {
        _baseDir = baseDir;
        Directory.CreateDirectory(_baseDir);
    }

    public string GetPath(string fileName) => Path.Combine(_baseDir, fileName);

    public async Task<string?> ReadTextAsync(string fileName)
    {
        string path = GetPath(fileName);
        return File.Exists(path) ? await File.ReadAllTextAsync(path) : null;
    }

    public async Task WriteTextAsync(string fileName, string content)
    {
        string path = GetPath(fileName);
        await File.WriteAllTextAsync(path, content);
    }

    public bool Exists(string fileName) => File.Exists(GetPath(fileName));

    public void Delete(string fileName)
    {
        string path = GetPath(fileName);
        if (File.Exists(path)) File.Delete(path);
    }
}