using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
using ConsoleRPG.Log;
using ConsoleRPG.World;
namespace ConsoleRPG.Items;

public abstract class Weapon : Item
{
    public int damage;
    protected abstract int _soundRange { get; }
    public override int GetDamage() => damage;
    public override void PickUp(Player p, Map map)
    {
        p.Inventory.Add(this);
        map.soundManager.Notify(p.X,p.Y,_soundRange,map);
        Cell cell = map.GetCell(p.X,p.Y);
        cell.Items.Remove(this);
        GameLogger.GetInstance().Log($"Picked up: {Name}.");
    }
}

public abstract class OneHandedWeapon : Weapon
{
    public override void EquipLeft(Player p,Item item)
    {
        p.UnequipLeft();
        p.LeftHand = item;
        p.Inventory.Remove(item);
        GameLogger.GetInstance().Log($"{item.Name} is in left hand");
    }
    public override void EquipRight(Player p, Item item)
    {
        p.UnequipRight();
        p.RightHand = item;
        p.Inventory.Remove(item);
       GameLogger.GetInstance().Log($"{item.Name} is in right hand");
    }
    public override void UnEquip(Player p, Item item)
    {
       if (p.LeftHand == item) p.LeftHand = null;
       if (p.RightHand == item) p.RightHand = null;
       p.Inventory.Add(item);
       GameLogger.GetInstance().Log($"Unequiped {item.Name}");
    }

}

public abstract class TwoHandedWeapon : Weapon
{
    private void EquipBoth(Player p, Item item)
    {
        p.UnequipLeft();
        p.UnequipRight();
        p.LeftHand = item;
        p.RightHand = item;
        p.Inventory.Remove(item);
       GameLogger.GetInstance().Log($"{Name} is in 2 hands now");
    }
    public override void EquipLeft(Player p, Item item) => EquipBoth(p,item);
    public override void EquipRight(Player p, Item item) => EquipBoth(p, item);
    public override void UnEquip(Player p, Item item)
    {
        p.LeftHand = null;
        p.RightHand = null;
        p.Inventory.Add(item);
        GameLogger.GetInstance().Log($"Unequiped {item.Name}.");
    }
   
} 
public abstract class HeavyWeapon : TwoHandedWeapon 
{
    protected override int _soundRange => 10;
    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);
}

public abstract class LightWeapon : OneHandedWeapon
{
    protected override int _soundRange => 3;
    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);
    public override void PickUp(Player p, Map map)
    {
        p.Inventory.Add(this);
        map.soundManager.Notify(p.X,p.Y,_soundRange,map);
        Cell cell = map.GetCell(p.X,p.Y);
        cell.Items.Remove(this);
        GameLogger.GetInstance().Log($"Picked up: {Name}.");
    }
}

public abstract class MagicWeapon : OneHandedWeapon 
{
    protected override int _soundRange => 5;
    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);
    public override void PickUp(Player p, Map map)
    {
        p.Inventory.Add(this);
        map.soundManager.Notify(p.X,p.Y,_soundRange,map);
        Cell cell = map.GetCell(p.X,p.Y);
        cell.Items.Remove(this);
        GameLogger.GetInstance().Log($"Picked up: {Name}.");
    }
}


public class Sword : LightWeapon { 
    public override char Symbol => '†'; 
    public override string Name => "Sword"; 
    public Sword() { damage = 6; }
}

public class Axe : HeavyWeapon { 
    public override char Symbol => 'P'; 
    public override string Name => "Axe"; 
    public Axe() { damage = 10; } 
}

public class MagicStaff : MagicWeapon { 
    public override char Symbol => '∫'; 
    public override string Name => "Magic Staff"; 
    public MagicStaff() { damage = 4; } 
}