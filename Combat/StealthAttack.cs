namespace ConsoleRPG.Combat;
using ConsoleRPG.Items;
using ConsoleRPG.Entities;

public class StealthAttack : IAttackVisitor
{
    public int ResultDamage { get;  set; }
     public int ResultDefense { get;  set; } 
    private Player _player;

    public StealthAttack(Player player)
    {
        _player = player;
        ResultDefense = 0; 
        ResultDamage = 0;
    }

    public void Visit(HeavyWeapon weapon)
    {
        ResultDamage = weapon.damage / 2;
        ResultDefense = _player.Strength;
    }

    public void Visit(LightWeapon weapon)
    {
        ResultDamage = (weapon.damage + _player.Dexterity + _player.Luck) * 2;
        ResultDefense = _player.Dexterity;
    }

    public void Visit(MagicWeapon weapon)
    {
        ResultDamage = 1;
        ResultDefense = 0;
    }
    public void Visit(Item item) 
    {
        ResultDamage = 0;
        ResultDefense = 0;
    }
}