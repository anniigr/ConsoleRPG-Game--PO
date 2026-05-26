using System.Collections.Generic;

namespace ConsoleRPG.Network
{
    public class NetworkMessage
    {
        public string Type { get; set; } = string.Empty; // "INIT", "UPDATE", "INPUT"
        public string Payload { get; set; } = string.Empty;
    }

    public class ConnectionInitDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<string> MapRows { get; set; } = new List<string>(); // Символы проходимости '#' и '.'
    }

    public class PlayerDto
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // ДОБАВЛЕННЫЕ СТАТЫ ДЛЯ ПАНЕЛИ
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }
        public int Coins { get; set; }
        public int Gold { get; set; }
        public string LeftHandName { get; set; } = string.Empty;
        public string RightHandName { get; set; } = string.Empty;
        public List<string> InventoryNames { get; set; } = new List<string>();
    }

    public class EnemyDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
    }

    public class ItemDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; } = string.Empty;
        public char Symbol { get; set; }
    }

    public class GameUpdateDto
    {
        public int YourPlayerId { get; set; }
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public List<EnemyDto> Enemies { get; set; } = new List<EnemyDto>();
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
        public string LastLog { get; set; } = string.Empty;
        public List<string> NewLogs { get; set; } = new List<string>();
    }
    public class PlayerStatsDto
    {
        public string Name { get; set; } = string.Empty;
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }
        public int Coins { get; set; }
        public int Gold { get; set; }
        public string RightHandItem { get; set; } = "Puste";
        public string LeftHandItem { get; set; } = "Puste";
        public int InventoryCount { get; set; }
    }

    // Класс всего состояния игры для отрисовки
    public class GameStateDto
    {
        public int PlayerId { get; set; }
        public List<string> GridRows { get; set; } = new List<string>();
        public PlayerStatsDto PlayerStats { get; set; } = new PlayerStatsDto();
        public List<string> RecentLogs { get; set; } = new List<string>();
    }
}