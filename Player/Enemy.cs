namespace ConsoleRPG.Entities;
public class Enemy
{
    public string Name { get; }
    public char Symbol { get; }
    public int Health {get; set;}
    public int AttackValue {get; private set;}
    public int Armor {get; private set;}

    public Enemy(string name, char symbol, int hp, int attack, int armor)
    {
        Name = name;
        Symbol = symbol;
        Health = hp;
        AttackValue = attack;
        Armor = armor;
    }
    public void TakeDamage (int damage) => Health -= int.Max(0, damage - Armor);
}