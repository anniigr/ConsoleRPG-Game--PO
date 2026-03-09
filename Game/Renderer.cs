using System;
using System.Text;
using ConsoleRPG.Entities;
using ConsoleRPG.World;

namespace ConsoleRPG.Engine
{
    public class Renderer
    {
        private Map map;
        private Player player;

        public Renderer(Map map, Player player)
        {
            this.map = map;
            this.player = player;
        }

        public void DrawFrame(bool isInventoryMode, int invCursor)
        {
            StringBuilder screen = new StringBuilder();
            Console.SetCursorPosition(0, 0);

            Cell playerCell = map.GetCell(player.X, player.Y);
            string floorInfo = playerCell.Items.Count > 0 ? $"Floor: {playerCell.Items[playerCell.Items.Count - 1].Name}, press [E] to pick up" : "Empty floor";

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (x == player.X && y == player.Y)
                        screen.Append('¶');
                    else
                        screen.Append(map.GetCell(x, y).GetDrawSymbol());
                }

                screen.Append("   | ");
                screen.Append(GetLine(y, floorInfo, isInventoryMode, invCursor));
                screen.AppendLine();
            }

            screen.AppendLine(new string('-', map.Width + 45));
            screen.AppendLine($"LOG: {player.LogMessage}".PadRight(80));

            Console.Write(screen.ToString());
        }

        private string GetLine(int lineIndex, string floorInfo, bool isInventoryMode, int invCursor)
        {
            string line = ""; 
            int panelWidth = 50; 

            if (isInventoryMode)
            {
                if (lineIndex == 0) line = "--- EQUIPMENT MODE ---";
                else if (lineIndex == 1) line = "[W/S] Choose | [L] Take to the left arm | [R] Take to the right arm ";
                else if (lineIndex == 2) line = "[Q] Throw it out | [1] Take off left | [2] Take off right";
                else if (lineIndex == 3) line = "[I] or [ESC] Close";
                else if (lineIndex == 4) line = "----------------------";
                else
                {
                    int itemIdx = lineIndex - 5;
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
                    8 => "--- ENVIROMENT ---",
                    9 => floorInfo,
                    _ => ""
                };
            }

            
            return line.PadRight(panelWidth);
        }
    }
}