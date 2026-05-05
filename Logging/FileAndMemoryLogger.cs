namespace ConsoleRPG.Log;
public class FileAndMemoryLogger : ILogger
{
    private readonly List<string> _logs = new List<string>();
    private readonly string _filePath;

    public FileAndMemoryLogger(string playerName, string configPath)
    {
        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _filePath = Path.Combine(configPath, $"{playerName}_{timestamp}_log.txt");
        
        Log("=== Rozpoczęto nową grę ===");
    }

    public void Log(string message)
    {
        string entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
        _logs.Add(entry);

        try { File.AppendAllText(_filePath, entry + Environment.NewLine); } 
        catch { }
    }

    public List<string> GetAllLogs() => _logs;
    
    public string GetLastLog() => _logs.Count > 0 ? _logs[^1] : "Brak zdarzeń.";
    
    public string GetLogFilePath() => _filePath;
}