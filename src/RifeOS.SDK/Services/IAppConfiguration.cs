namespace RifeOS.SDK.Services;

public interface IAppConfiguration
{
    T GetValue<T>(string key, T defaultValue);
    Task SetValueAsync<T>(string key, T value);
}
