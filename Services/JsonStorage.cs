using System.Text.Json;

namespace Infonet.Services;

public static class JsonStorage
{
    public static T? Load<T>(string path)
    {
        if (!File.Exists(path))
            return default;
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static void Save<T>(string path, T data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(path, json);
    }
}
