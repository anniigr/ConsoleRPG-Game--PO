namespace ConsoleRPG.Combat;
using ConsoleRPG.Items;
public interface IAttackVisitor
{
    int ResultDamage { get; set; }
    int ResultDefense { get; set; }
    void Visit(HeavyWeapon weapon);
    void Visit(LightWeapon weapon);
    void Visit(MagicWeapon weapon);
    void Visit(Item item);
}