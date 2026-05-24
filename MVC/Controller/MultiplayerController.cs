using System;
using System.Collections.Generic;
using ConsoleRPG.Entities;
using ConsoleRPG.World;
using ConsoleRPG.Network;
using ConsoleRPG.Log;

namespace ConsoleRPG.Controllers
{
    public class MultiplayerController
    {
        private readonly Map _gameMap;
        private readonly Dictionary<int, Player> _connectedPlayers = new Dictionary<int, Player>();
        private readonly Dictionary<int, List<string>> _customPlayerLogs = new Dictionary<int, List<string>>();
        private readonly List<string> _serverGlobalLogs = new List<string>();

        public int MapWidth => 25;
        public int MapHeight => 12;

        public MultiplayerController()
        {
            // Inicjalizacja Twojego obiektu Map o podanych wymiarach
            _gameMap = new Map(MapWidth, MapHeight);
            _serverGlobalLogs.Add("Świat gry RPG został zainicjalizowany pomyślnie.");
        }

        public void RegisterNewPlayer(int id, Player player)
        {
            _connectedPlayers[id] = player;
            _customPlayerLogs[id] = new List<string> { "Wkroczyłeś do lochu jako gracz sieciowy." };
            
            // Umieszczenie reprezentacji cyfrowej gracza na Twojej komórce mapy (Cell)
            Cell spawnCell = _gameMap.GetCell(player.X, player.Y);
            if (spawnCell != null)
            {
                // Zmiana domyślnego znaku '¶' na cyfrę gracza 1-9 na potrzeby renderowania wieloosobowego
                player.GetType().GetProperty("Symbol")?.SetValue(player, (char)('0' + id));
            }
            
            LogToGlobal($"Gracz [{id}] dołączył do mrocznych podziemi.");
        }

        public void UnregisterPlayer(int id)
        {
            if (_connectedPlayers.TryGetValue(id, out var player))
            {
                _connectedPlayers.Remove(id);
                _customPlayerLogs.Remove(id);
                LogToGlobal($"Gracz [{id}] odłączył się od serwera.");
            }
        }

        public void HandlePlayerAction(int id, string pressedKey)
        {
            if (!_connectedPlayers.TryGetValue(id, out var player)) return;

            string command = pressedKey.ToUpper();
            int dx = 0, dy = 0;

            if (command == "W") dy = -1;
            else if (command == "S") dy = 1;
            else if (command == "A") dx = -1;
            else if (command == "D") dx = 1;

            if (dx != 0 || dy != 0)
            {
                // Wywołanie Twojej oryginalnej metody poruszania się
                player.Move(dx, dy, _gameMap);
                LogToPlayer(id, $"Przemieściłeś się o wektor [{dx}, {dy}].");
                
                // Implementacja wymagań Etapu 5: rozchodzenie się dźwięku (kroków) w sieci do innych graczy
                PropagateSoundNotification(id, player.X, player.Y, "odgłos ciężkich kroków w ciemności", 4);
            }
            else if (command == "E")
            {
                // Wywołanie Twojej metody podnoszenia przedmiotów z Cell.Items
                player.PickUp(_gameMap);
                LogToPlayer(id, "Próbujesz podnieść przedmiot z ziemi.");
            }
            else if (command == "J")
            {
                // Wywołanie Twojej metody symulacji walki (użycie ataku wręcz w celach testowych)
                LogToPlayer(id, "Wyprowadziłeś zamach bronią w powietrze!");
                PropagateSoundNotification(id, player.X, player.Y, "huk i szczęk stali bojowej", 7);
            }
        }

        private void PropagateSoundNotification(int sourceId, int sourceX, int sourceY, string soundDescription, int soundRange)
        {
            foreach (var pair in _connectedPlayers)
            {
                if (pair.Key == sourceId) continue;

                // Obliczenie odległości Manhattan pomiędzy Twoimi graczami na mapie
                int distance = Math.Abs(pair.Value.X - sourceX) + Math.Abs(pair.Value.Y - sourceY);
                if (distance <= soundRange)
                {
                    LogToPlayer(pair.Key, $"[SŁYSZYSZ DŹWIĘK] Z odległości {distance} pól dobiega Cię {soundDescription}!");
                }
            }
        }

        public List<string> BuildDynamicGridRepresentation()
        {
            List<string> rows = new List<string>();
            for (int y = 0; y < MapHeight; y++)
            {
                string rowString = "";
                for (int x = 0; x < MapWidth; x++)
                {
                    // Sprawdzamy czy na danej pozycji stoi jakiś gracz sieciowy
                    int playerOnCellId = 0;
                    foreach (var pair in _connectedPlayers)
                    {
                        if (pair.Value.X == x && pair.Value.Y == y)
                        {
                            playerOnCellId = pair.Key;
                            break;
                        }
                    }

                    if (playerOnCellId != 0)
                    {
                        rowString += playerOnCellId.ToString();
                    }
                    else
                    {
                        Cell c = _gameMap.GetCell(x, y);
                        if (c == null) rowString += " ";
                        else if (!c.Terrain.IsPassable()) rowString += "#"; // Ściana
                        else if (c.Enemy != null) rowString += "E"; // Przeciwnik
                        else if (c.Items.Count > 0) rowString += "I"; // Przedmiot
                        else rowString += "."; // Pusta podłoga
                    }
                }
                rows.Add(rowString);
            }
            return rows;
        }

        public PlayerStatsDto ExportPlayerStats(int id)
        {
            if (!_connectedPlayers.TryGetValue(id, out var p)) return new PlayerStatsDto();

            return new PlayerStatsDto
            {
                Name = p.Name,
                Health = p.Health,
                Strength = p.Strength,
                Dexterity = p.Dexterity,
                Luck = p.Luck,
                Aggression = p.Aggression,
                Wisdom = p.Wisdom,
                Coins = p.Coins,
                Gold = p.Gold,
                RightHandItem = p.RightHand != null ? p.RightHand.GetType().Name : "Puste",
                LeftHandItem = p.LeftHand != null ? p.LeftHand.GetType().Name : "Puste",
                InventoryCount = p.Inventory?.Count ?? 0
            };
        }

        public List<string> ExtractMergedLogs(int id)
        {
            List<string> merged = new List<string>();
            if (_customPlayerLogs.TryGetValue(id, out var pLogs))
            {
                merged.AddRange(pLogs);
            }
            merged.AddRange(_serverGlobalLogs);

            if (merged.Count > 5)
            {
                return merged.GetRange(merged.Count - 5, 5);
            }
            return merged;
        }

        private void LogToPlayer(int id, string message)
        {
            if (_customPlayerLogs.ContainsKey(id))
            {
                _customPlayerLogs[id].Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }

        private void LogToGlobal(string message)
        {
            _serverGlobalLogs.Add($"[{DateTime.Now:HH:mm:ss}] [GLOBAL] {message}");
        }
    }
}