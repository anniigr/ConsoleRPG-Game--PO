using System;
using System.Security.Cryptography;
using System.Text;
using ConsoleRPG.Entities;
using ConsoleRPG.World;

namespace ConsoleRPG.Engine
{
    public class Renderer
    {
        private Map map;
        private Player player;
        private ActionManager actionManager;

        public Renderer(Map map, Player player, ActionManager actionManager)
        {
            this.map = map;
            this.player = player;
            this.actionManager = actionManager;
        }

       public void DrawFrame(GameEngine engine)
        {
            
            bool isInventoryMode = engine.isInventoryOpened;
            int invCursor = engine.InventoryCursor;

            StringBuilder screen = new StringBuilder();
            Console.SetCursorPosition(0, 0);

            Cell playerCell = map.GetCell(player.X, player.Y);
            string floorInfo = "Empty floor";

            if (playerCell != null)
            {
                if (playerCell.Items.Count > 0)
                    floorInfo = "Press [E] to pick up";
                else
                    floorInfo = "Empty floor";
            }

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (x == player.X && y == player.Y)
                        screen.Append('¶');
                    else
                    {
                        Cell cell = map.GetCell(x, y);
                        screen.Append(cell != null ? cell.GetDrawSymbol() : '#');
                    }
                }

                screen.Append("   | ");
                screen.Append(GetLine(y, floorInfo, isInventoryMode, invCursor, engine));
                screen.AppendLine();
            }

            screen.AppendLine(new string('-', map.Width + 45));
            screen.AppendLine($"LOG: {player.LogMessage}".PadRight(80));

            Console.Write(screen.ToString());
            
        }

       private string GetLine(int lineIndex, string floorInfo, bool isInventoryMode, int invCursor, GameEngine engine)
       {
            string line = ""; 
            int panelWidth = 50; 
            if (engine.IsHelpOpened)
            {
                return GetHelpLine(lineIndex, engine).PadRight(panelWidth);
            }

            if (isInventoryMode)
            {
                if (lineIndex == 0) line = "--- EQUIPMENT MODE ---";
                else if (lineIndex == 1) line = "[H] Help";
                else
                {
                    int itemIdx = lineIndex - 2 ;
                    if (itemIdx >= 0 && itemIdx < player.Inventory.Count)
                    {
                        string pointer = (itemIdx == invCursor) ? "> " : "  ";
                        line = $"{pointer}{player.Inventory[itemIdx].Name}";
                    }
                }
            }
            else
            {
                line = lineIndex switch
                {
                    0 => "--- STATISTICS ---",
                    1 => $"HP: {player.Health} | Strength: {player.Strength} | Dexterity: {player.Dexterity}",
                    2 => $"Luck: {player.Luck} | Aggression: {player.Aggression} | Wisdom: {player.Wisdom}",
                    3 => $"Coins: {player.Coins} | Gold: {player.Gold}",
                    4 => "--- EQUIPMENT (Press I) ---",
                    5 => $"Left arm:  {(player.LeftHand != null ? player.LeftHand.Name : "[Empty]")}",
                    6 => $"Right arm: {(player.RightHand != null ? player.RightHand.Name : "[Empty]")}",
                    7 => $"Damage:  {player.GetTotalDamage()}",
                    8 => "--- ENVIRONMENT ---",
                    9 => floorInfo,
                    10 => "[H] Help",

                    _ => ""
                };
            }

            
            return line.PadRight(panelWidth);
        }
        private string GetHelpLine(int lineIndex, GameEngine engine)
        {
            var actions = actionManager.GetAvailableActions(engine)
                .Select(a => $"[{FormatKey(a.key)}] {ShortName(a.Name)}")
                .ToList();

            if (lineIndex == 0) return "--- HELP ---";
            if (lineIndex == 1) return "Available actions:";
            if (lineIndex == 2) return "";

            int actionIndex = lineIndex - 3;

            if (actionIndex >= 0 && actionIndex < actions.Count)
                return actions[actionIndex];

            if (lineIndex == actions.Count + 3)
                return "";

            if (lineIndex == actions.Count + 4)
                return "Press [H] to close";

            return "";
        }
        private string FormatKey(ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.D0 => "0",
                ConsoleKey.D1 => "1",
                ConsoleKey.D2 => "2",
                ConsoleKey.D3 => "3",
                ConsoleKey.D4 => "4",
                ConsoleKey.D5 => "5",
                ConsoleKey.D6 => "6",
                ConsoleKey.D7 => "7",
                ConsoleKey.D8 => "8",
                ConsoleKey.D9 => "9",
                _ => key.ToString()
            };
        }
        private string ShortName(string name)
        {
            return name switch
            {
                "Move player/cursor up" => "Move up",
                "Move player/cursor down" => "Move down",
                "Move/Equipt left" => "Move left",
                "Move/Equipt right" => "Move right",
                "Inventory Switch" => "Inventory",
                "Quit Game / throw down" => "Quit/Drop",
                "PickUp" => "Pick up",
                "Equipt left" => "Equip left",
                "Equipt right" => "Equip right",
                "Unequip Left" => "Unequip left",
                "UnequipRight" => "Unequip right",
                _ => name
            };
        }

    }
}