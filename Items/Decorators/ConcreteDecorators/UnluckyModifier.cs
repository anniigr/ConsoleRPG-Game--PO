using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
using ConsoleRPG.World;
namespace ConsoleRPG.Items;

public class UnluckyModifier : ItemDecorator
{
    public UnluckyModifier(Item item) : base(item){}
    public override char Symbol => _innerItem.Symbol;
    public override string Name => _innerItem.Name + "(Unlucky)";
    public override void EquipLeft(Player p, Item self) 
    {
        _innerItem.EquipLeft(p, self); 
        p.Luck -= 5;                   
    }

    public override void EquipRight(Player p, Item self) 
    {
        _innerItem.EquipRight(p, self);
        p.Luck -= 5;
    }

    public override void UnEquip(Player p, Item self) 
    {
        _innerItem.UnEquip(p, self);
        p.Luck += 5;
    }
    public override void Accept(IAttackVisitor visitor)
    {
        _innerItem.Accept(visitor); 
    }
}