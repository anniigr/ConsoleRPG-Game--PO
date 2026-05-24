using ConsoleRPG.World;
using ConsoleRPG.Entities;
using System.Collections.Generic;

namespace ConsoleRPG.Systems
{
    public class EnemySystem
    {
        public void ProcessTurn(Map map)
        {
            List<Enemy> allEnemies = new List<Enemy>();
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.GetCell(x, y).Enemy != null)
                    {
                        allEnemies.Add(map.GetCell(x, y).Enemy);
                    }
                }
            }

            foreach (var enemy in allEnemies)
            {
                enemy.MoveRandomly(map);
            }
        }
    }
}