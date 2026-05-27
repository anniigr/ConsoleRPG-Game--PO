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
            GameConfig config = ConfigManager.LoadConfig();
            _engine = new GameEngine(config, isServer: true);
        }

        public void StartServer()
        {
            _isRunning = true;
            _listener.Start();
            GameLogger.GetInstance().Log($"[SERWER] Uruchomiony na porcie {_port}. Oczekiwanie na graczy...");
            Task.Run(AcceptClientsAsync);
            Task.Run(EnemyLoopAsync);
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

                        int spawnX = 2, spawnY = 2;
                        while (_engine.map.GetCell(spawnX, spawnY) != null && !_engine.map.GetCell(spawnX, spawnY).Terrain.IsPassable())
                        {
                            spawnX++;
                        }

                        var newPlayer = new Player(spawnX, spawnY) { Name = $"Gracz {id}", Id = id };
                        _engine.map.soundManager.Subscribe(newPlayer); 
                        _engine.players[id] = newPlayer;

                        Task.Run(() => HandleClientAsync(id, client));
                        
                        SendMapInitialization(id, client);
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
                            if (_engine.players.ContainsKey(id) && _engine.players[id].IsDead)
                            {
                                var deathMsg = new NetworkMessage { Type = "DEATH", Payload = "Zginąłeś w lochach!" };
                                var deathWriter = new StreamWriter(client.GetStream()) { AutoFlush = true };
                                deathWriter.WriteLine(JsonSerializer.Serialize(deathMsg));
                                break;
                            }
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
                    if (_engine.players.ContainsKey(id))
                    {
                        _engine.map.soundManager.Unsubscribe(_engine.players[id]); // <-- Обязательно отписываем
                        _engine.players.Remove(id);
                    }
                    _clients.Remove(id);
                    client.Close();
                }
                BroadcastState();
            }
        }

        private void BroadcastState()
        {
            lock (_lock) 
            {
            foreach (var pair in _clients)
            {
                int id = pair.Key;
                TcpClient client = pair.Value;
                if (!client.Connected) continue;

                var updateDto = new GameUpdateDto { YourPlayerId = id };
                updateDto.NewLogs = GameLogger.GetInstance().ExtractMergedLogs(id);
                updateDto.LastLog = updateDto.NewLogs.Count > 0 ? updateDto.NewLogs[^1] : "Brak zdarzeń.";

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
        }

        public void TerminateServer()
        {
            _isRunning = false;
            _listener.Stop();
        }
        private async Task EnemyLoopAsync()
        {
            while (_isRunning)
            {
                await Task.Delay(1500); 
                lock (_lock)
                {
                    _engine.ProcessEnemyTurn();
                }
                BroadcastState();
            }
        }
        
    }
}