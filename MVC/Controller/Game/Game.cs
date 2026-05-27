using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleRPG.Entities;
using ConsoleRPG.Config;
using ConsoleRPG.World;
using ConsoleRPG.Log;
using ConsoleRPG.Systems;
using ConsoleRPG.MVC.View;
using ConsoleRPG.Network;
using ConsoleRPG.Items;

namespace ConsoleRPG.Engine
{
    public class GameEngine 
    {
        public IGameState CurrentState => _currentState;
        public Map map { get; private set; }
        public Dictionary<int, Player> players { get; private set; } = new Dictionary<int, Player>(); 
        public int playerId;
        public Player player => players.ContainsKey(playerId) ? players[playerId] : null!;
        public bool isServer;
        public bool IsRunning { get; private set; } = true;


        private ActionManager _actionManager;
        private IGameState _currentState;
        private Dictionary<int, IGameState> _playerStates = new Dictionary<int, IGameState>();
        private EnemySystem _enemySystem;

        public GameEngine(GameConfig config, bool isServer)
        {
            this.isServer = isServer;
            _actionManager = new ActionManager();
            _enemySystem = new EnemySystem();
            _currentState = new MapState();

            if (isServer)
            {
                IDungeonBuilder debugBuilder = new LoggingDungeonBuilder();
                var director = new DungeonDirector(debugBuilder);
                GameLogger.GetInstance(config.PlayerName, config.FileName);
                IDungeonThemeFactory currentTheme = ThemeSelector.SelectTheme(config.Theme);
                GameLogger.GetInstance().Log(currentTheme.GetGreetingMessage());

                map = director.CreateThemeMap(currentTheme);
            }
            
        }
        public void Quit()
        {
            IsRunning = false; 
            GameLogger.GetInstance().Log("Gra zakończona przez gracza.");
        }
        public void ChangeState(IGameState newGameState) 
        {
            _currentState = newGameState;
        }

        public void ExcecuteAction(ConsoleKey key)
        {
            var action = _actionManager.FindAction(key);
            if (action != null && action.IsExecutable(this))
            {
                action.Execute(this);
            }
            else
            {
                GameLogger.GetInstance().Log($"Wciśnięto nieznany lub zablokowany klawisz: {key}");
            }
        }

        public void ProcessEnemyTurn()
        {
            if (isServer)
            {
                _enemySystem.ProcessTurn(this.map);
            }
        }

        public string GetFloorInfo()
        {
            if (player == null || map == null) return "Void";
            
            var cell = map.GetCell(player.X, player.Y);
            if (cell == null) return "Void";
            
            if (cell.Items.Count > 0)
            {
                return cell.Items.Count > 1 
                    ? $"Items here ({cell.Items.Count})" 
                    : $"Item: {cell.Items[0].Name}";
            }
            
            return "Empty floor";
        }
        public void ExecuteActionForPlayer(int id, ConsoleKey key)
        {
            lock (this)
            {
                if (!players.ContainsKey(id)) return;
                this.playerId = id; 
                GameLogger.GetInstance().CurrentPlayerId = id;

                if (isServer)
                {
                    if (!_playerStates.ContainsKey(id)) _playerStates[id] = new MapState();
                    _currentState = _playerStates[id];
                }
                _currentState.Update(this, key);

                if (isServer) _playerStates[id] = _currentState; 
                
                GameLogger.GetInstance().CurrentPlayerId = 0;
                }
        }

        public void LoadMapFromServer(ConnectionInitDto init)
        {
            map = new Map(init.Width, init.Height);
            for (int y = 0; y < init.Height; y++)
            {
                for (int x = 0; x < init.Width; x++)
                {
                    Terrain terrain = (init.MapRows[y][x] == '#') ? new Wall() : new Floor();
                    map.SetCell(x, y, new Cell(terrain));
                }
            }
        }

        public void SyncFromServer(GameUpdateDto update)
        {
            this.playerId = update.YourPlayerId;
            if (update.NewLogs != null)
            {
                GameLogger.GetInstance().SetLogs(update.NewLogs);
            }

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var cell = map.GetCell(x, y);
                    if (cell != null)
                    {
                        cell.Enemy = null;
                        cell.Items.Clear();
                    }
                }
            }

            foreach (var pDto in update.Players)
            {
                if (!players.ContainsKey(pDto.Id))
                {
                    players[pDto.Id] = new Player(pDto.X, pDto.Y) { Name = pDto.Name , Id = pDto.Id};
                }
                var p = players[pDto.Id];
                p.X = pDto.X; p.Y = pDto.Y; p.Health = pDto.Health;
                
                p.Strength = pDto.Strength; p.Dexterity = pDto.Dexterity; p.Luck = pDto.Luck;
                p.Aggression = pDto.Aggression; p.Wisdom = pDto.Wisdom;
                p.Coins = pDto.Coins; p.Gold = pDto.Gold;
                
                p.LeftHand = string.IsNullOrEmpty(pDto.LeftHandName) ? null : new GenericItem(pDto.LeftHandName, ' ');
                p.RightHand = string.IsNullOrEmpty(pDto.RightHandName) ? null : new GenericItem(pDto.RightHandName, ' ');
                
                p.Inventory.Clear();
                foreach (var itemName in pDto.InventoryNames) p.Inventory.Add(new GenericItem(itemName, 'I'));
            }

            var activeIds = update.Players.Select(p => p.Id).ToHashSet();
            var toRemove = players.Keys.Where(id => !activeIds.Contains(id)).ToList();
            foreach (var id in toRemove) players.Remove(id);

            // Синхронизация врагов
            foreach (var eDto in update.Enemies)
            {
                var cell = map.GetCell(eDto.X, eDto.Y);
                if (cell != null)
                {
                    cell.Enemy = new SimpleEnemy("Potwór", eDto.Symbol, eDto.X, eDto.Y, 50, 10, 2);
                }
            }

            foreach (var iDto in update.Items)
            {
                var cell = map.GetCell(iDto.X, iDto.Y);
                if (cell != null)
                {
                    cell.Items.Add(new GenericItem(iDto.Name, iDto.Symbol));
                }
            }

        }
    }
}