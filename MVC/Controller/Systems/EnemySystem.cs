using ConsoleRPG.World;
using ConsoleRPG.Entities;
using System.Collections.Generic;

namespace ConsoleRPG.Systems
{
    public class EnemySystem
    {
        public void ProcessTurn(Map map, Dictionary<int, Player> players)
        {
            List<Enemy> allEnemies = new List<Enemy>();
            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                    if (map.GetCell(x, y)?.Enemy != null)
                        allEnemies.Add(map.GetCell(x, y).Enemy);

            List<Entity> playerEntities = players.Values.Cast<Entity>().ToList();

            foreach (var enemy in allEnemies)
            {
                enemy.ProcessMove(map, playerEntities);
            }
        }
    }
}