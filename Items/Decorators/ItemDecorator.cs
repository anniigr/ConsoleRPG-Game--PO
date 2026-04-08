using ConsoleRPG.Entities;
using ConsoleRPG.World;
using ConsoleRPG.Combat;
namespace ConsoleRPG.Items;
public abstract class ItemDecorator : Item
{
    protected Item _innerItem;
    protected ItemDecorator (Item item)
    {
        _innerItem = item;
    }
    public override char Symbol => _innerItem.Symbol;
    public override string Name => _innerItem.Name;
    public override void Accept(IAttackVisitor visitor) => _innerItem.Accept(visitor);
    public override int GetDamage() => _innerItem.GetDamage();
    

    public override void EquipLeft(Player p, Item item) => _innerItem.EquipLeft(p, item);
    public override void EquipRight(Player p, Item item) => _innerItem.EquipRight(p, item);
    public override void UnEquip(Player p, Item item) => _innerItem.UnEquip(p, item);


    public override void EquipLeft(Player p) => _innerItem.EquipLeft(p, this);
    public override void EquipRight(Player p) => _innerItem.EquipRight(p, this);
    public override void UnEquip(Player p) => _innerItem.UnEquip(p, this);
}