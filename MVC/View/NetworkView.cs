using System;
using ConsoleRPG.Network;

namespace ConsoleRPG.Views
{
    public class NetworkView
    {
        private readonly object _consoleLock = new object();

        public void Render(GameStateDto state)
        {
            lock (_consoleLock)
            {
                Console.SetCursorPosition(0, 0);
                
                Console.WriteLine("==========================================================================");
                Console.WriteLine($"                 CONSOLE RPG MULTIPLAYER - TWÓJ NUMER: {state.PlayerId} ");
                Console.WriteLine("==========================================================================");

                Console.WriteLine("\n[WIDOK MAPY LOCHU]");
                foreach (var row in state.GridRows)
                {
                    Console.WriteLine(row);
                }

                Console.WriteLine("\n[STATYSTYKI TWOJEGO BOHATERA]");
                var stats = state.PlayerStats;
                Console.WriteLine($"HP: {stats.Health} | Złoto: {stats.Gold} | Monety: {stats.Coins}");
                Console.WriteLine($"Siła: {stats.Strength} | Zręczność: {stats.Dexterity} | Szczęście: {stats.Luck} | Agresja: {stats.Aggression} | Mądrość: {stats.Wisdom}");
                Console.WriteLine($"Broń (Prawa ręka): {stats.RightHandItem} | Tarcza/Przedmiot (Lewa ręka): {stats.LeftHandItem}");
                Console.WriteLine($"Liczba przedmiotów w plecaku (Inventory): {stats.InventoryCount}");

                Console.WriteLine("\n[DZIENNIK ZDARZEŃ I ROZCHODZENIA SIĘ DŹWIĘKU]");
                foreach (var log in state.RecentLogs)
                {
                    Console.WriteLine(log);
                }
                Console.WriteLine("==========================================================================");
                Console.Write("Ruch: W, A, S, D | Podnoszenie: E | Atak: J. Wykonaj akcję: ");
            }
        }
    }
}