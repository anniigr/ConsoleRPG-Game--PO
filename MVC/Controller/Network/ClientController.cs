using System;
using System.Threading.Tasks;
using ConsoleRPG.MVC.View;

namespace ConsoleRPG.Networking
{
    public class ClientController
    {
        private readonly GameClient _client;
        private IGameView? _view;

        public ClientController(GameClient client)
        {
            _client = client;
        }

        public async Task RunLoopAsync()
        {
            while (_client.IsActive)
            {
                if (_client.Engine.map != null)
                {
                    if (_view == null) 
                    {
                        _view = new ConsoleView(_client.Engine.map, _client.Engine.players);
                    }
                    lock (_client.StateLock) 
                    {
                        _client.Engine.CurrentState.Draw(_view, _client.Engine);
                    }
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    _client.SendInput(key);

                if (_client.Engine.playerId != 0) 
                {
                    _client.Engine.ExecuteActionForPlayer(_client.Engine.playerId, key);
                }
                }
                await Task.Delay(20);
            }
        }
    }
}