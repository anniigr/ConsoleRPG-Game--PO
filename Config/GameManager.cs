using System.Text.Json;
using ConsoleRPG.World;
namespace ConsoleRPG.Config;
public static class ConfigManager
{
    private const string _configFilePath = "config.json";
    public static GameConfig LoadConfig()
    {
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true,
            WriteIndented = true 
        };
        if (!File.Exists(_configFilePath))
        {
            var defaultConfig = new GameConfig();
            string json = JsonSerializer.Serialize(defaultConfig, options);
            File.WriteAllText(_configFilePath,json);
        }
        

        string jsonString = File.ReadAllText(_configFilePath);
        try
        {
            return JsonSerializer.Deserialize<GameConfig>(jsonString, options) ?? new GameConfig();
        }
        catch
        {
            return new GameConfig();
        }
    }
}