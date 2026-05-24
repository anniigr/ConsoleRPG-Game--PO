using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConsoleRPG.Network;
using ConsoleRPG.Engine;
using ConsoleRPG.Config;

namespace ConsoleRPG.Networking
{
    public class GameClient
    {
        private readonly string _ip;
        private readonly int _port;
        private TcpClient? _socket;
        private bool _isActive;
        private GameEngine _engine;

        public GameClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
            
            // Локальный движок-пустышка для клиента, карту загрузим из сети
            GameConfig config = new GameConfig();
            _engine = new GameEngine(config, isServer: false);
        }

        public async Task RunClientAsync()
        {
            try
            {
                _socket = new TcpClient();
                await _socket.ConnectAsync(_ip, _port);
                _isActive = true;

                Console.Clear();
                Console.WriteLine("[KLIENT] Połączono! Pobieranie mapy z serwera...");

                // Запускаем асинхронное чтение обновлений от сервера
                _ = Task.Run(ReceiveUpdatesAsync);

                // Основной поток занимается исключительно отправкой нажатых клавиш
                SendInputLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BŁĄD] Serwer niedostępny: {ex.Message}");
                Console.ReadKey();
            }
        }

        private async Task ReceiveUpdatesAsync()
        {
            if (_socket == null) return;
            var reader = new StreamReader(_socket.GetStream());

            while (_isActive && _socket.Connected)
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
                        if (initData != null) _engine.LoadMapFromServer(initData);
                    }
                    else if (msg.Type == "UPDATE")
                    {
                        var updateData = JsonSerializer.Deserialize<GameUpdateDto>(msg.Payload);
                        if (updateData != null)
                        {
                            // Синхронизируем и автоматически перерисовываем карту через ConsoleView
                            _engine.SyncFromServer(updateData);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("\nPołączenie z serwerem przerwane.");
                    break;
                }
            }
            _isActive = false;
        }

        private void SendInputLoop()
        {
            if (_socket == null) return;
            var writer = new StreamWriter(_socket.GetStream()) { AutoFlush = true };

            while (_isActive)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (_engine.playerId != 0) 
                    {
                        _engine.ExecuteActionForPlayer(_engine.playerId, keyInfo.Key);
                    }
                    var msg = new NetworkMessage { Type = "INPUT", Payload = keyInfo.Key.ToString() };
                    try
                    {
                        writer.WriteLine(JsonSerializer.Serialize(msg));
                    }
                    catch { break; }
                }
                Thread.Sleep(20);
            }
        }
    }
}