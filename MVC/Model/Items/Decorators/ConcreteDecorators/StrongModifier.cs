using ConsoleRPG.Entities;
using ConsoleRPG.World;
using ConsoleRPG.Combat;
namespace ConsoleRPG.Items;

public class StrongModifier : ItemDecorator
{
    public StrongModifier(Item item) : base(item){}
    public override char Symbol => _innerItem.Symbol;
    public override string Name => _innerItem.Name + "(Strong)";

    public override int GetDamage() => _innerItem.GetDamage() + 5;
    public override void EquipLeft(Player p, Item self) => _innerItem.EquipLeft(p, self);
    public override void EquipRight(Player p, Item self) => _innerItem.EquipRight(p, self);
    public override void UnEquip(Player p, Item self) => _innerItem.UnEquip(p, self);
    public override void Accept(IAttackVisitor visitor)
    {
        
        _innerItem.Accept(visitor); 
        //visitor.ResultDamage += 5; 
    }
}