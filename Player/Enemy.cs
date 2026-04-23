namespace ConsoleRPG.Entities;
public class Enemy : Entity
{
    public int AttackValue {get; private set;}
    public int Armor {get; private set;}

    public Enemy(string name, char symbol, int x, int y, int hp, int attack, int armor) : base (name,symbol,x,y,hp)
    {
        AttackValue = attack;
        Armor = armor;
    }
    public override void TakeDamage (int damage) => Health -= int.Max(0, damage - Armor);
}