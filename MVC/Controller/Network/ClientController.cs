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
                // 1. ОТРИСОВКА: Заставляем текущее состояние (Карта, Инвентарь или Помощь) рисовать себя
                if (_client.Engine.map != null)
                {
                    if (_view == null) 
                    {
                        _view = new ConsoleView(_client.Engine.map, _client.Engine.players);
                    }
                    lock (_client.StateLock) // DODANE: Blokujemy model na czas rysowania widoku
                    {
                        _client.Engine.CurrentState.Draw(_view, _client.Engine);
                    }
                }

                // 2. ОБРАБОТКА ВВОДА
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    _client.SendInput(key);

                // 3. Локальное предсказание (Client-side prediction):
                // Мы сразу применяем действие локально, чтобы игра не "тупила" в ожидании ответа от сети.
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