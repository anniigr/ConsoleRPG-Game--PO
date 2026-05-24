namespace ConsoleRPG.Log;
public interface ILogger
{
    void Log(string message);
    List<string> GetAllLogs();
    string GetLastLog();
    string GetLogFilePath();
}

public class FileLogger : ILogger
{
    private string _filePath;
    public FileLogger(string playerName, string configPath) {
        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _filePath = Path.Combine(configPath, $"{playerName}_{timestamp}_log.txt");
     
    }
    public void Log(string message) => File.AppendAllText(_filePath, message + "\n");
    public string GetLastLog() => "Zapisano do pliku"; 
    public List<string> GetAllLogs() => new List<string> { "Logi w pliku: " + _filePath };
    public string GetLogFilePath() => _filePath;
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG]: {message}");
    public string GetLastLog() => "Wyświetlono в konsoli";
    public List<string> GetAllLogs() => new List<string> { "Tylko podgląd konsoli" };
    public string GetLogFilePath() => "BRAK (Konsola)";
}