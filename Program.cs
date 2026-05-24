using System;
using System.Threading.Tasks;
using ConsoleRPG.Networking;
using ConsoleRPG.Controllers;

namespace ConsoleRPG
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string systemMode = "";
            int operationalPort = 5555;
            string remoteIpTarget = "127.0.0.1";

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--server")
                    {
                        systemMode = "SERVER";
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int extractedPort))
                        {
                            operationalPort = extractedPort;
                        }
                    }
                    else if (args[i] == "--client")
                    {
                        systemMode = "CLIENT";
                        if (i + 1 < args.Length)
                        {
                            string[] connectionSegments = args[i + 1].Split(':');
                            remoteIpTarget = connectionSegments[0];
                            if (connectionSegments.Length > 1 && int.TryParse(connectionSegments[1], out int extractedPort))
                            {
                                operationalPort = extractedPort;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(systemMode))
            {
                Console.Clear();
                Console.WriteLine("==========================================================================");
                Console.WriteLine("                    GRA RPG - REFAKTORYZACJA SIECIOWA                     ");
                Console.WriteLine("==========================================================================");
                Console.Write("Wybierz tryb działania aplikacji -> (S)erwer / (K)lient: ");
                
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                Console.WriteLine();
                
                if (pressedKey == ConsoleKey.S) systemMode = "SERVER";
                else if (pressedKey == ConsoleKey.K) systemMode = "CLIENT";
                else
                {
                    Console.WriteLine("[BŁĄD] Wybrano nieobsługiwany klawisz wyboru. Zamykanie programu.");
                    return;
                }
            }

            if (systemMode == "SERVER")
            {
                GameServer server = new GameServer(operationalPort);
                server.StartServer();

                Console.WriteLine("[INFO] Serwer działa w tle. Naciśnij klawisz [ESC], aby zamknąć aplikację serwera.");
                while (true)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        server.TerminateServer();
                        break;
                    }
                    await Task.Delay(250);
                }
            }
            else if (systemMode == "CLIENT")
            {
                GameClient client = new GameClient(remoteIpTarget, operationalPort);
                await client.RunClientAsync();
            }
        }
    }
}