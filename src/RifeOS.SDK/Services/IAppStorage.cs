namespace RifeOS.SDK.Services;

public interface IAppStorage
{
    string GetPath(string fileName);
    Task<string?> ReadTextAsync(string fileName);
    Task WriteTextAsync(string fileName, string content);
    bool Exists(string fileName);
    void Delete(string fileName);
}