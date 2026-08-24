using System.IO;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class AppStorageService : IAppStorage
{
    public string DataDirectory { get; }

    public AppStorageService(string appId)
    {
        DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Sandbox", appId);
        EnsureCreated();
    }

    public string GetDatabaseFilePath(string databaseFileName = "data.db")
    {
        return Path.Combine(DataDirectory, databaseFileName);
    }

    public void EnsureCreated()
    {
        if (!Directory.Exists(DataDirectory))
        {
            Directory.CreateDirectory(DataDirectory);
        }
    }
}
