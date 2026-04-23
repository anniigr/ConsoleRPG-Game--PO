using System;
using ConsoleRPG.Entities;
using ConsoleRPG.Config;
using ConsoleRPG.World;

namespace ConsoleRPG.Engine
{
    public class GameEngine 
    {
        public Map map {get; private set;}
        public Player player {get; private set;}
        private Renderer _renderer;
        private ActionManager _actionManager;
        private IGameState _currentState;

        private bool _isRunning = true;

        public GameEngine(GameConfig config)
        {
            IDungeonBuilder debugBuilder = new LoggingDungeonBuilder();
            var director = new DungeonDirector(debugBuilder);

            GameLogger.GetInstance(config.PlayerName, config.FileName);
            IDungeonThemeFactory currentTheme = ThemeSelector.SelectTheme(config.Theme);
            GameLogger.GetInstance().Log(currentTheme.GetGreetingMessage());

            map = director.CreateThemeMap(currentTheme);
            player = new Player(0,0);
            _currentState = new MapState();
            _renderer = new Renderer(map, player);
            _actionManager = new ActionManager();
        }
        public void ChangeState (IGameState newGameState) => _currentState = newGameState;
        public void Quit () =>_isRunning = false;
        public void ExcecuteAction (ConsoleKey key)
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
        public void Run()
        {
            while (_isRunning)
            {
                _currentState.Draw(_renderer,this);
                var key = Console.ReadKey(true).Key;
                _currentState.Update(this,key);
            }
        }
        public string GetFloorInfo()
        {
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

}
}