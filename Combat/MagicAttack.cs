namespace ConsoleRPG.Combat;
using ConsoleRPG.Items;
using ConsoleRPG.Entities;

public class MagicAttack : IAttackVisitor
{
    public int ResultDamage { get;  set; }
    
    private Player _player;
    public int ResultDefense { get;  set; } 

    public MagicAttack(Player player)
    {
        _player = player;
        ResultDefense = _player.Luck; 
        ResultDamage = 0;
    }

    public void Visit(HeavyWeapon weapon)
    {
        ResultDamage = 1;
        ResultDefense = _player.Luck;
        
    }

    public void Visit(LightWeapon weapon)
    {
        ResultDamage = 1;
        ResultDefense = _player.Luck;
    }

    public void Visit(MagicWeapon weapon)
    {
        ResultDamage = weapon.damage + (_player.Wisdom * 2);
        ResultDefense = _player.Wisdom * 2;
    }
    public void Visit(Item item) 
    {
        ResultDamage = 0;
        ResultDefense = _player.Luck;
    }
}