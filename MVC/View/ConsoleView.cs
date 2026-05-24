using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ConsoleRPG.Entities;
using ConsoleRPG.Engine;
using ConsoleRPG.World;
using ConsoleRPG.Log;

namespace ConsoleRPG.MVC.View
{
    public class ConsoleView : IGameView
    {
        private Map _map;
        private Dictionary<int, Player> _players; // Храним всех игроков

        public ConsoleView(Map map, Dictionary<int, Player> players)
        {
            _map = map;
            _players = players;
        }

        public void DrawFrame(GameEngine engine, IGameState currentState)
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder screen = new StringBuilder();

            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    // 1. Ищем, стоит ли на этих координатах (x,y) какой-нибудь игрок
                    Player? foundPlayer = null;
                    int foundPlayerId = 0;

                    foreach (var pair in _players)
                    {
                        if (pair.Value.X == x && pair.Value.Y == y)
                        {
                            foundPlayerId = pair.Key;
                            foundPlayer = pair.Value;
                            break; // Игрок найден, дальше искать не нужно
                        }
                    }

                    // 2. Если игрок найден — рисуем его ID (например, '1' или '2')
                    if (foundPlayer != null)
                    {
                        screen.Append(foundPlayerId.ToString()); 
                    }
                    else
                    {
                       var cell = _map.GetCell(x, y);
                        if (cell != null && cell.Items.Count > 0)
                        {
                            // Берем символ последнего предмета в списке
                            screen.Append(cell.Items.Last().Symbol); 
                        }
                        else
                        {
                            // Если нет ни игрока, ни предметов — рисуем ландшафт
                            screen.Append(cell?.GetDrawSymbol() ?? '#');
                        }
                }
                }

                screen.Append("   | ");
                
                string line = currentState.GetPanelLine(y, engine);
                screen.Append(line.PadRight(50)); 
                
                screen.AppendLine();
            }
            
            Console.Write(screen.ToString());
        }

        public void DrawLog()
        {
            // Обрати внимание: теперь тут _map вместо map
            int logLine = _map.Height + 1; 
            
            Console.SetCursorPosition(0, logLine);
            Console.Write(new string(' ', Console.WindowWidth)); 
            
            Console.SetCursorPosition(0, logLine);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"> {GameLogger.GetInstance().GetLastLog()}");
            Console.ResetColor();
        }

        public void DrawHistory(List<string> logs)
        {
            Console.Clear();
            Console.WriteLine("=== HISTORIA ZDARZEŃ (Wciśnij J aby wrócić) ===");
            
            int start = Math.Max(0, logs.Count - 20);
            for (int i = start; i < logs.Count; i++)
            {
                Console.WriteLine(logs[i]);
            }
        }
    }
}