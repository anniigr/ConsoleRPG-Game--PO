using System;
using System.Text;
using System.Linq;
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

        public void DrawFrame(GameEngine engine, IGameState currentState)
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder screen = new StringBuilder();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (x == player.X && y == player.Y) screen.Append('¶');
                    else screen.Append(map.GetCell(x, y)?.GetDrawSymbol() ?? '#');
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
            int logLine = map.Height + 1; 
            
            Console.SetCursorPosition(0, logLine);
            Console.Write(new string(' ', Console.WindowWidth)); 
            
            Console.SetCursorPosition(0, logLine);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"> {GameLogger.GetInstance().GetLastLog()}");
            Console.ResetColor();
        }

    }
}