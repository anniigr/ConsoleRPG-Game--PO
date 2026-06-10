using ConsoleRPG.World;
using ConsoleRPG.Log;

namespace ConsoleRPG.Entities;

public class ChasePlayerBehaviour : IVisionBehaviour
{
    public void OnPlayerSeen(Enemy enemy, List<Entity> visiblePlayers, Map map)
    {
        if (visiblePlayers.Count == 0) return;

        Entity target = visiblePlayers
            .OrderBy(p => Math.Abs(p.X - enemy.X) + Math.Abs(p.Y - enemy.Y))
            .First();

        int dx = Math.Sign(target.X - enemy.X);
        int dy = Math.Sign(target.Y - enemy.Y);

        if (Math.Abs(target.X - enemy.X) + Math.Abs(target.Y - enemy.Y) == 1)
        {
            if (target is Player player)
            {
                int dmg = Math.Max(0, enemy.AttackValue - player.Dexterity/4);
                player.Health -= dmg;
                GameLogger.GetInstance().Log($"{enemy.Name} atacks {player.Name} with {dmg} damage!");
            }
            return;
        }

        if (dx != 0 && TryMove(enemy, dx, 0, map)) return;
        if (dy != 0 && TryMove(enemy, 0, dy, map)) return;
    }

    private bool TryMove(Enemy enemy, int dx, int dy, Map map)
    {
        int nx = enemy.X + dx;
        int ny = enemy.Y + dy;
        if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) return false;
        var cell = map.GetCell(nx, ny);
        if (cell == null || !cell.Terrain.IsPassable() || cell.Enemy != null) return false;
        map.GetCell(enemy.X, enemy.Y).Enemy = null;
        enemy.X = nx;
        enemy.Y = ny;
        map.GetCell(nx, ny).Enemy = enemy;
        return true;
    }
}

public class FleePlayerBehaviour : IVisionBehaviour
{
    public void OnPlayerSeen(Enemy enemy, List<Entity> visiblePlayers, Map map)
    {
        if (visiblePlayers.Count == 0) return;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { -1, 1, 0, 0 };

        var forbiddenDirs = new HashSet<(int, int)>();
        foreach (var p in visiblePlayers)
        {
            int fx = Math.Sign(p.X - enemy.X);
            int fy = Math.Sign(p.Y - enemy.Y);
            if (fx != 0) forbiddenDirs.Add((fx, 0));
            if (fy != 0) forbiddenDirs.Add((0, fy));
        }

        for (int i = 0; i < 4; i++)
        {
            if (forbiddenDirs.Contains((dx[i], dy[i]))) continue;
            int nx = enemy.X + dx[i];
            int ny = enemy.Y + dy[i];
            if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) continue;
            var cell = map.GetCell(nx, ny);
            if (cell != null && cell.Terrain.IsPassable() && cell.Enemy == null)
            {
                map.GetCell(enemy.X, enemy.Y).Enemy = null;
                enemy.X = nx;
                enemy.Y = ny;
                map.GetCell(nx, ny).Enemy = enemy;
                return;
            }
        }
    }
}

public class IgnorePlayerBehaviour : IVisionBehaviour
{
    public void OnPlayerSeen(Enemy enemy, List<Entity> visiblePlayers, Map map) { }
}