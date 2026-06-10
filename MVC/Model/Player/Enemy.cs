using ConsoleRPG.Entities.Observers;
using ConsoleRPG.Log;
using ConsoleRPG.World;

namespace ConsoleRPG.Entities;
public abstract class Enemy : Entity, IEventListenerDeath, IEventListenerSound
{
    public int AttackValue {get;  set;}
    public int Armor {get;  set;}
    public EventManagerDeath group;
    public EventManagerSound enemiesManagerSound;

    public ISoundBehaviour SoundBehaviour { get; set; } = new IgnoreSoundBehaviour();
    public IVisionBehaviour VisionBehaviour { get; set; } = new IgnorePlayerBehaviour();
    public bool HasSoundTarget { get; set; } = false;
    public int SoundTargetX { get; set; }
    public int SoundTargetY { get; set; }
    public int VisionRange { get; set; } = 5;

    public abstract void MemberDied(string name);
    public Map? CurrentMap { get; set; }
    public void SoundProduced(int dist, int sourceX, int sourceY)
    {
        GameLogger.GetInstance().Log($"{this.Name} heard sound from ({sourceX}, {sourceY})");
        if (CurrentMap != null)
            SoundBehaviour.OnSoundHeard(this, sourceX, sourceY, CurrentMap);
    }

    public void ProcessMove(Map map, List<Entity> allPlayers)
    {
        var visiblePlayers = GetPlayersInSight(map, allPlayers);
        if (visiblePlayers.Count > 0)
        {
            VisionBehaviour.OnPlayerSeen(this, visiblePlayers, map);
            HasSoundTarget = false; 
            return;
        }

        if (HasSoundTarget)
        {
            bool moved = MoveTowardSound(map);
            if (X == SoundTargetX && Y == SoundTargetY)
                HasSoundTarget = false;
            if (moved) return;
            HasSoundTarget = false;
        }

        MoveRandomly(map);
    }
    private bool MoveTowardSound(Map map)
    {
        if (SoundBehaviour is ChaseSoundBehaviour)
        {
            int dx = Math.Sign(SoundTargetX - X);
            int dy = Math.Sign(SoundTargetY - Y);
            if (dx != 0 && TryMoveDir(dx, 0, map)) return true;
            if (dy != 0 && TryMoveDir(0, dy, map)) return true;
            return false;
        }
        else if (SoundBehaviour is FleeSoundBehaviour)
        {
            int[] dxs = { 0, 0, 1, -1 };
            int[] dys = { -1, 1, 0, 0 };
            int forbidX = Math.Sign(SoundTargetX - X);
            int forbidY = Math.Sign(SoundTargetY - Y);
            for (int i = 0; i < 4; i++)
            {
                if (dxs[i] == forbidX && dys[i] == 0) continue;
                if (dxs[i] == 0 && dys[i] == forbidY) continue;
                if (TryMoveDir(dxs[i], dys[i], map)) return true;
            }
            HasSoundTarget = false;
            return false;
        }
        return false;
    }

    protected bool TryMoveDir(int dx, int dy, Map map)
    {
        int nx = X + dx;
        int ny = Y + dy;
        if (nx < 0 || nx >= map.Width || ny < 0 || ny >= map.Height) return false;
        var cell = map.GetCell(nx, ny);
        if (cell == null || !cell.Terrain.IsPassable() || cell.Enemy != null) return false;
        map.GetCell(X, Y).Enemy = null;
        X = nx;
        Y = ny;
        map.GetCell(X, Y).Enemy = this;
        return true;
    }

    private List<Entity> GetPlayersInSight(Map map, List<Entity> allPlayers)
    {
        var result = new List<Entity>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { -1, 1, 0, 0 };

        foreach (var player in allPlayers)
        {
            for (int d = 0; d < 4; d++)
            {
                for (int step = 1; step <= VisionRange; step++)
                {
                    int cx = X + dx[d] * step;
                    int cy = Y + dy[d] * step;
                    if (cx < 0 || cx >= map.Width || cy < 0 || cy >= map.Height) break;
                    var cell = map.GetCell(cx, cy);
                    if (cell == null || !cell.Terrain.IsPassable()) break; 
                    if (player.X == cx && player.Y == cy)
                    {
                        result.Add(player);
                        break;
                    }
                }
            }
        }
        return result;
    }

    public void MoveRandomly(Map map)
    {
        int[] dx = {0, 0, 1, -1};
        int[] dy = {-1, 1, 0, 0};
        Random rng = Rng.Instance;

        for (int attempt = 0; attempt < 5; attempt++)
        {
            int i = rng.Next(4);
            int nx = X + dx[i];
            int ny = Y + dy[i];

            if (nx >= 0 && nx < map.Width && ny >= 0 && ny < map.Height && 
                map.GetCell(nx, ny).Terrain.IsPassable() && 
                map.GetCell(nx, ny).Enemy == null) 
                {
                    map.GetCell(X, Y).Enemy = null; 
                    X = nx;
                    Y = ny;
                    map.GetCell(X, Y).Enemy = this; 
                    break;
                }
        }
    }

    public Enemy(string name, char symbol, int x, int y, int hp, int attack, int armor) : base (name,symbol,x,y,hp)
    {
        AttackValue = attack;
        Armor = armor;
    }
    public override void TakeDamage (int damage)
    {
        Health -= int.Max(0, damage - Armor);
        if (Health <= 0) 
        {
            Died();
        }
    }

    private void Died()
    {
        group.Unsubscribe(this);
        group.Notify();
        enemiesManagerSound.Unsubscribe(this);
    }
}
public class Goblin : Enemy
{
    public Goblin(int x, int y) : base("Goblin", 'g', x, y, 20, 9, 2)
    {
        SoundBehaviour = new FleeSoundBehaviour();
        VisionBehaviour = new FleePlayerBehaviour();
    }
    public override void MemberDied(string name)
    {
        this.AttackValue -= 2;
        this.Armor -= 1;
        GameLogger.GetInstance().Log($"Goblin испугался. Его атака снизилась до {AttackValue}");
    }
}

public class Zombie : Enemy
{
    public Zombie(int x, int y) : base("Zombie", 'z', x, y, 30, 10, 2)
    {
        SoundBehaviour = new ChaseSoundBehaviour();
        VisionBehaviour = new ChasePlayerBehaviour();
    }
    public override void MemberDied(string name)
    {
        this.AttackValue -= 8;
        this.Armor -= 1;
        GameLogger.GetInstance().Log($"Зомби впал в отчаяние. Его атака снизилась до {AttackValue}");
    }
}

public class Skeleton : Enemy
{
    private bool _wasAttacked = false;

    public Skeleton(int x, int y) : base("Skeleton", 's', x, y, 38, 10, 2)
    {
        SoundBehaviour = new IgnoreSoundBehaviour();
        VisionBehaviour = new IgnorePlayerBehaviour();
    }

    public void ReactToAttack(int maxHealth)
    {
        if (_wasAttacked) return;
        _wasAttacked = true;

        if (Health >= maxHealth / 2)
        {
            SoundBehaviour = new ChaseSoundBehaviour();
            VisionBehaviour = new ChasePlayerBehaviour();
            GameLogger.GetInstance().Log($"{Name} get angry and start chasing!");
        }
        else
        {
            SoundBehaviour = new FleeSoundBehaviour();
            VisionBehaviour = new FleePlayerBehaviour();
            GameLogger.GetInstance().Log($"{Name} frightened and start running away!");
        }
    }

    public override void MemberDied(string name)
    {
        this.AttackValue += 8;
        GameLogger.GetInstance().Log($"Skeleton in rage. Attack increased to {AttackValue}");
    }

    public override void TakeDamage(int damage)
    {
        int maxHp = 38;
        base.TakeDamage(damage);
        if (!_wasAttacked) ReactToAttack(maxHp);
    }
}

public class SimpleEnemy : Enemy 
{
    public SimpleEnemy(string name, char symbol, int x, int y, int hp, int attack, int armor) 
        : base(name, symbol, x, y, hp, attack, armor) { }
        public override void MemberDied(string name)
    {
        this.AttackValue += 0 ;
        GameLogger.GetInstance().Log($"Skeleton is in furror. His AttackValue encreased up to {AttackValue}");
    }
}