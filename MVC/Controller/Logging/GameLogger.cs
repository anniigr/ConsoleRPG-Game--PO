namespace ConsoleRPG.Log;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameLogger : ILogger
{
    // Храним логи строго в хронологическом порядке: (id_игрока, сообщение)
    // Если playerId == 0, значит это глобальное событие
    private readonly List<(int playerId, string message)> _chronologicalLogs = new();
    
    private static GameLogger _instance;
    public int CurrentPlayerId { get; set; } = 0;

    private GameLogger() { }

    public static GameLogger GetInstance(string name = "Player", string path = "./")
    {
        if (_instance == null) _instance = new GameLogger();
        return _instance;
    }

    public void Log(string message)
    {
        // Проверяем, есть ли уже дата (чтобы на клиенте не двоилось время)
        string entry = message.StartsWith("[") ? message : $"[{DateTime.Now:HH:mm:ss}] {message}";
        _chronologicalLogs.Add((CurrentPlayerId, entry));
    }
    // Метод для отправки лога конкретному игроку, независимо от того, чей сейчас ход
    public void LogToSpecificPlayer(int targetPlayerId, string message)
    {
        string entry = message.StartsWith("[") ? message : $"[{DateTime.Now:HH:mm:ss}] {message}";
        _chronologicalLogs.Add((targetPlayerId, entry));
    }

    public List<string> ExtractMergedLogs(int id)
    {
        // Фильтруем логи: только глобальные (0) или личные для этого игрока
        var filteredLogs = _chronologicalLogs
            .Where(log => log.playerId == 0 || log.playerId == id)
            .Select(log => log.message)
            .ToList();
            
        // Возвращаем только последние 20 сообщений, чтобы не перегружать сеть!
        if (filteredLogs.Count > 20)
        {
            return filteredLogs.GetRange(filteredLogs.Count - 20, 20);
        }
        return filteredLogs;
    }

    // Добавленный метод для КЛИЕНТА: синхронизация готовой истории от сервера
    public void SetLogs(List<string> logs)
    {
        _chronologicalLogs.Clear();
        foreach (var log in logs)
        {
            _chronologicalLogs.Add((0, log)); 
        }
    }

    // Обязательные методы интерфейса
    public List<string> GetAllLogs() => _chronologicalLogs.Select(l => l.message).ToList(); 
    public string GetLastLog() 
    {
        var logs = GetAllLogs();
        return logs.Count > 0 ? logs[^1] : "Brak zdarzeń.";
    }
    public string GetLogFilePath() => ""; 
}