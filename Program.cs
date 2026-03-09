using System;
using ConsoleRPG.Engine;

namespace ConsoleRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear(); 
            Console.CursorVisible = false;
            Console.Title = "OOP Console RPG - Stage 1";
            
            GameEngine engine = new GameEngine();
            engine.Run();
        }
    }
}
