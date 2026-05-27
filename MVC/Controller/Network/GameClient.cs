using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleRPG.Network;
using ConsoleRPG.Engine;
using ConsoleRPG.Config;
using ConsoleRPG.Log;

namespace ConsoleRPG.Networking
{
    public class GameClient
    {
        private readonly string _ip;
        private readonly int _port;
        private TcpClient? _socket;
        public bool IsActive { get; private set; }
        public GameEngine Engine { get; private set; }
        public readonly object StateLock = new object();

        public GameClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
            GameConfig config = ConfigManager.LoadConfig();
            Engine = new GameEngine(config, isServer: false);
        }

        public async Task RunClientAsync()
        {
            try
            {
                _socket = new TcpClient();
                await _socket.ConnectAsync(_ip, _port);
                IsActive = true;

                GameLogger.GetInstance().Log("[KLIENT] Połączono! Pobieranie mapy z serwera...");
                
                _ = Task.Run(ReceiveUpdatesAsync);
            }
            catch (Exception ex)
            {
                GameLogger.GetInstance().Log($"[BŁĄD] Serwer niedostępny: {ex.Message}");
                IsActive = false;
            }
        }

        private async Task ReceiveUpdatesAsync()
        {
            if (_socket == null) return;
            var reader = new StreamReader(_socket.GetStream());

            while (IsActive && _socket.Connected)
            {
                try
                {
                    string? line = await reader.ReadLineAsync();
                    if (line == null) break;

                    var msg = JsonSerializer.Deserialize<NetworkMessage>(line);
                    if (msg == null) continue;

                    if (msg.Type == "INIT")
                    {
                        var initData = JsonSerializer.Deserialize<ConnectionInitDto>(msg.Payload);
                        if (initData != null) Engine.LoadMapFromServer(initData);
                    }
                    else if (msg.Type == "UPDATE")
                    {
                        var updateData = JsonSerializer.Deserialize<GameUpdateDto>(msg.Payload);
                        if (updateData != null)
                        {
                            lock (StateLock) 
                            {
                                Engine.SyncFromServer(updateData);
                            }
                        }
                    }
                    else if (msg.Type == "DEATH") 
                    {
                        GameLogger.GetInstance().Log("\n=== GAME OVER ===");
                        GameLogger.GetInstance().Log(msg.Payload);
                        IsActive = false;
                        break;
                    }
                }
                catch
                {
                    IsActive = false;
                    break;
                }
            }
            IsActive = false;
        }

        public void SendInput(ConsoleKey key)
        {
            if (_socket == null || !IsActive) return;
            var writer = new StreamWriter(_socket.GetStream()) { AutoFlush = true };
            var msg = new NetworkMessage { Type = "INPUT", Payload = key.ToString() };
            try
            {
                writer.WriteLine(JsonSerializer.Serialize(msg));
            }
            catch 
            { 
                IsActive = false; 
            }
        }
    }
}