using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleRPG.Network;
using ConsoleRPG.Entities;
using ConsoleRPG.Engine;
using ConsoleRPG.Config;
using ConsoleRPG.Log;

namespace ConsoleRPG.Networking
{
    public class GameServer
    {
        private readonly int _port;
        private readonly TcpListener _listener;
        private readonly Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();
        private readonly GameEngine _engine;
        private readonly object _lock = new object();
        private bool _isRunning;

        public GameServer(int port)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Any, _port);
            
            // Инициализируем главный серверный движок с оригинальной картой
            GameConfig config = new GameConfig();
            _engine = new GameEngine(config, isServer: true);
        }

        public void StartServer()
        {
            _isRunning = true;
            _listener.Start();
            Console.WriteLine($"[SERWER] Uruchomiony na porcie {_port}. Oczekiwanie na graczy...");
            Task.Run(AcceptClientsAsync);
        }

        private async Task AcceptClientsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    lock (_lock)
                    {
                        if (_clients.Count >= 9) { client.Close(); continue; }

                        int id = 1;
                        while (_clients.ContainsKey(id)) id++;

                        _clients[id] = client;

                        // Спавним игрока на серверной карте в проходимой точке
                        int spawnX = 2, spawnY = 2;
                        while (_engine.map.GetCell(spawnX, spawnY) != null && !_engine.map.GetCell(spawnX, spawnY).Terrain.IsPassable())
                        {
                            spawnX++;
                        }

                        _engine.players[id] = new Player(spawnX, spawnY) { Name = $"Gracz {id}" };

                        Task.Run(() => HandleClientAsync(id, client));
                        
                        // 1. Отправляем карту новому клиенту
                        SendMapInitialization(id, client);
                        // 2. Рассылаем всем новые позиции
                        BroadcastState();
                    }
                }
                catch { break; }
            }
        }

        private void SendMapInitialization(int id, TcpClient client)
        {
            var initDto = new ConnectionInitDto
            {
                Width = _engine.map.Width,
                Height = _engine.map.Height
            };

            for (int y = 0; y < _engine.map.Height; y++)
            {
                string row = "";
                for (int x = 0; x < _engine.map.Width; x++)
                {
                    row += _engine.map.GetCell(x, y).Terrain.IsPassable() ? '.' : '#';
                }
                initDto.MapRows.Add(row);
            }

            var msg = new NetworkMessage { Type = "INIT", Payload = JsonSerializer.Serialize(initDto) };
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            writer.WriteLine(JsonSerializer.Serialize(msg));
        }

        private async Task HandleClientAsync(int id, TcpClient client)
        {
            var reader = new StreamReader(client.GetStream());
            try
            {
                while (_isRunning && client.Connected)
                {
                    string? line = await reader.ReadLineAsync();
                    if (line == null) break;

                    var msg = JsonSerializer.Deserialize<NetworkMessage>(line);
                    if (msg != null && msg.Type == "INPUT")
                    {
                        ConsoleKey key = Enum.Parse<ConsoleKey>(msg.Payload);
                        lock (_lock)
                        {
                            _engine.ExecuteActionForPlayer(id, key);
                            _engine.ProcessEnemyTurn();
                        }
                        BroadcastState();
                    }
                }
            }
            catch { }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(id);
                    _engine.players.Remove(id);
                    client.Close();
                }
                BroadcastState();
            }
        }

        private void BroadcastState()
        {
            foreach (var pair in _clients)
            {
                int id = pair.Key;
                TcpClient client = pair.Value;
                if (!client.Connected) continue;

                var updateDto = new GameUpdateDto { YourPlayerId = id };
                updateDto.LastLog = GameLogger.GetInstance().GetLastLog();

                // Собираем игроков
                foreach (var p in _engine.players)
                {
                    updateDto.Players.Add(new PlayerDto { 
                    Id = p.Key, X = p.Value.X, Y = p.Value.Y, Health = p.Value.Health, Name = p.Value.Name,
                    Strength = p.Value.Strength, Dexterity = p.Value.Dexterity, Luck = p.Value.Luck,
                    Aggression = p.Value.Aggression, Wisdom = p.Value.Wisdom,
                    Coins = p.Value.Coins, Gold = p.Value.Gold,
                    LeftHandName = p.Value.LeftHand?.Name ?? "",
                    RightHandName = p.Value.RightHand?.Name ?? "",
                    InventoryNames = p.Value.Inventory.Select(i => i.Name).ToList() // Передаем инвентарь
                });
                }

                // Собираем динамические объекты с карты
                for (int y = 0; y < _engine.map.Height; y++)
                {
                    for (int x = 0; x < _engine.map.Width; x++)
                    {
                        var cell = _engine.map.GetCell(x, y);
                        if (cell == null) continue;
                        if (cell.Enemy != null)
                            updateDto.Enemies.Add(new EnemyDto { X = x, Y = y, Symbol = cell.Enemy.Symbol });
                        foreach (var item in cell.Items)
                            updateDto.Items.Add(new ItemDto { X = x, Y = y, Name = item.Name,Symbol = item.Symbol });
                    }
                }

                try
                {
                    var msg = new NetworkMessage { Type = "UPDATE", Payload = JsonSerializer.Serialize(updateDto) };
                    var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    writer.WriteLine(JsonSerializer.Serialize(msg));
                }
                catch { }
            }
        }

        public void TerminateServer()
        {
            _isRunning = false;
            _listener.Stop();
        }
    }
}