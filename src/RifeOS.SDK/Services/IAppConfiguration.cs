namespace RifeOS.SDK.Services;

public interface IAppConfiguration
{
    T? Get<T>(string key, T? defaultValue = default);
    void Set<T>(string key, T value);
    Task SaveAsync();
}