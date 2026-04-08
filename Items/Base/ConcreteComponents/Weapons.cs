using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
namespace ConsoleRPG.Items;

public abstract class Weapon : Item
{
    public int damage;
    public override int GetDamage() => damage;
}

public abstract class OneHandedWeapon : Weapon
{
    public override void EquipLeft(Player p,Item item)
    {
        p.UnequipLeft();
        p.LeftHand = item;
        p.Inventory.Remove(item);
        p.LogMessage = $"{item.Name} is in left hand";
    }
    public override void EquipRight(Player p, Item item)
    {
        p.UnequipRight();
        p.RightHand = item;
        p.Inventory.Remove(item);
        p.LogMessage = $"{item.Name} is in right hand";
    }
    public override void UnEquip(Player p, Item item)
    {
       if (p.LeftHand == item) p.LeftHand = null;
       if (p.RightHand == item) p.RightHand = null;
       p.Inventory.Add(item);
       p.LogMessage = $"Unequiped {item.Name}";
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
        p.LogMessage = $"{Name} is in 2 hands now";
    }
    public override void EquipLeft(Player p, Item item) => EquipBoth(p,item);
    public override void EquipRight(Player p, Item item) => EquipBoth(p, item);
    public override void UnEquip(Player p, Item item)
    {
        p.LeftHand = null;
        p.RightHand = null;
        p.Inventory.Add(item);
        p.LogMessage = $"Unequiped {item.Name}.";
    }
   
} 
public abstract class HeavyWeapon : TwoHandedWeapon 
{
     public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);
}

public abstract class LightWeapon : OneHandedWeapon 
{
    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);
}

public abstract class MagicWeapon : OneHandedWeapon 
{
    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);
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