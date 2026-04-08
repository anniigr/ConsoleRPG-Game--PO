namespace ConsoleRPG.Combat;
using ConsoleRPG.Items;
using ConsoleRPG.Entities;

public class NormalAttack : IAttackVisitor
{
    public int ResultDamage { get;  set; }
    public int ResultDefense { get;  set; }
    private Player _player;

    public NormalAttack(Player player)
    {
        _player = player;
        ResultDefense = _player.Dexterity; 
        ResultDamage = 0;
    }

    public void Visit(HeavyWeapon weapon)
    {
        ResultDamage = weapon.damage + (_player.Strength / 2) + (_player.Aggression / 2);
        ResultDefense = _player.Strength + _player.Luck;
    }

    public void Visit(LightWeapon weapon)
    {
        ResultDamage = weapon.damage + (_player.Dexterity / 2);
        ResultDefense = _player.Dexterity + _player.Luck;
    }

    public void Visit(MagicWeapon weapon)
    {
        ResultDamage = 1;
        ResultDefense = _player.Dexterity + _player.Luck;
    }
    public void Visit(Item item) 
    {
        ResultDamage = 0;
        ResultDefense = _player.Dexterity;
    }
}