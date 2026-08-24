using RifeOS.SDK.Services;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;

namespace RifeOS.Host.Services;

public sealed class AppConfigurationService : IAppConfiguration
{
    private readonly string _configFilePath;
    private readonly ConcurrentDictionary<string, JsonElement> _configs = new();

    public AppConfigurationService(string configFilePath)
    {
        _configFilePath = configFilePath;
        Load();
    }

    private void Load()
    {
        if (File.Exists(_configFilePath))
        {
            try
            {
                string json = File.ReadAllText(_configFilePath);
                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (dict != null)
                {
                    foreach (var kvp in dict) _configs[kvp.Key] = kvp.Value;
                }
            }
            catch { }
        }
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (_configs.TryGetValue(key, out var element))
        {
            try { return element.Deserialize<T>(); }
            catch { return defaultValue; }
        }
        return defaultValue;
    }

    public void Set<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        _configs[key] = JsonDocument.Parse(json).RootElement;
    }

    public async Task SaveAsync()
    {
        var dir = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(_configs, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_configFilePath, json);
    }
}