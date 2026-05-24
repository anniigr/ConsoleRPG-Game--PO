namespace ConsoleRPG.Entities;
public abstract class Entity
{
    public string Name { get; set;}
    public char Symbol { get; }
    public int X {get;  set;}
    public int Y {get;  set;}

    public int Health { get; set; } = 100;        

    public Entity (string name, char symbol, int x, int y, int health)
    {
        Name = name;
        Symbol = symbol;
        X = x;
        Y = y;
        Health = health;
    }
    public virtual void TakeDamage (int damage) => Health -= int.Max(0, damage);
    public bool IsDead => Health <= 0;
} 