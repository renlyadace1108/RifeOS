using System.IO;
using System.Text.Json;
using RifeOS.SDK.Services;

namespace RifeOS.Host.Services;

public sealed class AppConfigurationService : IAppConfiguration
{
    private readonly string _configFilePath;
    private Dictionary<string, JsonElement> _cache = new();

    public AppConfigurationService(string appId)
    {
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Config", appId);
        Directory.CreateDirectory(dir);
        _configFilePath = Path.Combine(dir, "settings.json");
        Load();
    }

    private void Load()
    {
        if (!File.Exists(_configFilePath)) return;
        try
        {
            var json = File.ReadAllText(_configFilePath);
            _cache = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
        }
        catch
        {
            _cache = new();
        }
    }

    public T GetValue<T>(string key, T defaultValue)
    {
        if (_cache.TryGetValue(key, out var element))
        {
            try
            {
                return JsonSerializer.Deserialize<T>(element.GetRawText()) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    public async Task SetValueAsync<T>(string key, T value)
    {
        var rawJson = JsonSerializer.Serialize(value);
        using var doc = JsonDocument.Parse(rawJson);
        _cache[key] = doc.RootElement.Clone();

        var output = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_configFilePath, output);
    }
}
