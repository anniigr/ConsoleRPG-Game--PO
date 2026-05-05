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
    public abstract void MemberDied(string name);
    public void SoundProduced(int dist,int sourceX, int sourceY)
    {
        GameLogger.GetInstance().Log($"{this.Name} na pozycji ({this.X}, {this.Y}) usłyszał hałas ze źródła ({sourceX}, {sourceY}) w odległości {dist}");
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
    public Goblin(int x, int y) : base("Goblin",'g',x,y,20,9,2)
    {
    }
    public override void MemberDied(string name)
    {
        this.AttackValue -=2;
        this.Armor -= 1;
        GameLogger.GetInstance().Log($"Goblin is afraid. His AttackValue decreased to {AttackValue}");
    }
}
public class Zombie : Enemy
{
    public Zombie (int x, int y) : base("Zombie",'z',x,y,30,10,2)
    {
    }
    public override void MemberDied(string name)
    {
        this.AttackValue -= 8 ;
        this.Armor -= 1;
        GameLogger.GetInstance().Log($"Zombi is in dispair. His AttackValue decreased to {AttackValue}");
    }
}
public class Skeleton : Enemy
{
    public Skeleton (int x, int y) : base("Skeleton",'s',x,y,38,10,2)
    {
    }
    public override void MemberDied(string name)
    {
        this.AttackValue += 8 ;
        GameLogger.GetInstance().Log($"Skeleton is in furror. His AttackValue encreased up to {AttackValue}");
    }
}