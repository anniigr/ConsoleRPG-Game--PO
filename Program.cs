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
            Console.Title = "OOP Console RPG - Stage 2";
            
            ShowIntro();
            GameEngine engine = new GameEngine();
            engine.Run();
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