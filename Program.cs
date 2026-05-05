using System;
using ConsoleRPG.Engine;
using ConsoleRPG.Config;
using ConsoleRPG.Log;

namespace ConsoleRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear(); 
            Console.CursorVisible = false;
            Console.Title = "OOP Console RPG - Stage 2";
            
            ShowIntro();
            GameConfig config = ConfigManager.LoadConfig();
            GameEngine engine = new GameEngine(config);
            
            engine.Run();
            if (engine.player.IsDead)
            {
                Console.Clear();
                Console.WriteLine("==============================================");
                Console.WriteLine("                  GAME OVER                   ");
                Console.WriteLine("==============================================");
                Console.WriteLine("\nTwoja postać poległa w walce.");
                Console.WriteLine($"\nZapisano dziennik zdarzeń w pliku:\n{GameLogger.GetInstance().GetLogFilePath()}");
                Console.WriteLine("\nNaciśnij dowolny klawisz, aby wyjść...");
                Console.ReadKey(true);
            }
        }
    


    static void ShowIntro()
    {
        Console.Clear();

        Console.WriteLine("==============================================");
        Console.WriteLine("               CONSOLE RPG                    ");
        Console.WriteLine("==============================================");
        Console.WriteLine();
        Console.WriteLine("Witaj, wędrowcze.");
        Console.WriteLine("Przed Tobą mroczne korytarze, porzucone przedmioty");
        Console.WriteLine("i broń, która może zwiększyć Twoją siłę.");
        Console.WriteLine();
        Console.WriteLine("Twoim zadaniem jest przemierzać loch,");
        Console.WriteLine("zbierać przedmioty, zakładać ekwipunek");
        Console.WriteLine("i przetrwać jak najdłużej.");
        Console.WriteLine();
        Console.WriteLine("Naciśnij dowolny klawisz, aby rozpocząć przygodę...");
            
        Console.ReadKey(true);
        Console.Clear();
    }
    }
}