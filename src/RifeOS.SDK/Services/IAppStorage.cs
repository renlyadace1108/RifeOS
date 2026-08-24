namespace RifeOS.SDK.Services;

public interface IAppStorage
{
    string DataDirectory { get; }
    string GetDatabaseFilePath(string databaseFileName = "data.db");
    void EnsureCreated();
}
