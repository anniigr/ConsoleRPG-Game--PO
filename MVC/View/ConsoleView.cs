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
        private Dictionary<int, Player> _players; 

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
                    Player? foundPlayer = null;
                    int foundPlayerId = 0;

                    foreach (var pair in _players)
                    {
                        if (pair.Value.X == x && pair.Value.Y == y)
                        {
                            foundPlayerId = pair.Key;
                            foundPlayer = pair.Value;
                            break; 
                        }
                    }

                    if (foundPlayer != null)
                    {
                        screen.Append(foundPlayerId.ToString()); 
                    }
                    else
                    {
                       var cell = _map.GetCell(x, y);
                        if (cell != null && cell.Items.Count > 0)
                        {
                            screen.Append(cell.Items.Last().Symbol); 
                        }
                        else
                        {
                            screen.Append(cell?.GetDrawSymbol() ?? '#');
                        }
                }
                }

                screen.Append("   | ");
                
                string line = GeneratePanelLine(y, engine, currentState);
                screen.Append(line.PadRight(50)); 
                
                screen.AppendLine();
            }
            
            Console.Write(screen.ToString());
        }

        public void DrawLog()
        {
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
        private string GeneratePanelLine(int lineIndex, GameEngine engine, IGameState currentState)
        {
            string stateName = currentState.GetType().Name;

            if (stateName == "MapState")
            {
                return lineIndex switch
                {
                    0 => "--- STATISTICS ---",
                    1 => $"HP: {engine.player?.Health} | Strength: {engine.player?.Strength} | Dexterity: {engine.player?.Dexterity}",
                    2 => $"Luck: {engine.player?.Luck} | Aggression: {engine.player?.Aggression} | Wisdom: {engine.player?.Wisdom}",
                    3 => $"Coins: {engine.player?.Coins} | Gold: {engine.player?.Gold}",
                    4 => "--- EQUIPMENT (Press I) ---",
                    5 => $"Left arm:  {(engine.player?.LeftHand != null ? engine.player.LeftHand.Name : "[Empty]")}",
                    6 => $"Right arm: {(engine.player?.RightHand != null ? engine.player.RightHand.Name : "[Empty]")}",
                    7 => $"Damage:  {engine.player?.GetTotalDamage()}",
                    8 => "--- ENVIRONMENT ---",
                    9 => engine.GetFloorInfo(),
                    10 => "[H] Help",
                    _ => ""
                };
            }
            else if (stateName == "InventoryState")
            {
                var invState = currentState as InventoryState;
                if (lineIndex == 0) return "--- EQUIPMENT MODE ---";
                
                int itemIdx = lineIndex - 2;
                if (engine.player != null && itemIdx >= 0 && itemIdx < engine.player.Inventory.Count)
                {
                    string pointer = (invState != null && itemIdx == invState.InventoryCursor) ? "> " : "  ";
                    return $"{pointer}{engine.player.Inventory[itemIdx].Name}";
                }
                if (engine.player != null && lineIndex == engine.player.Inventory.Count + 3) return "[H] Help";
                return "";
            }
            else if (stateName == "HelpState")
            {
                var actions = currentState.GetActionManager().GetAvailableActions(engine);
                if (lineIndex == 0) return "--- HELP ---";
                int actionIdx = lineIndex - 2;
                if (actionIdx >= 0 && actionIdx < actions.Count) 
                    return $"[{actions[actionIdx].Key}] {actions[actionIdx].Name}";
                return "";
            }

            return "";
        }
    }
}