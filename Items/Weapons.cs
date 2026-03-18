using ConsoleRPG.Entities;
namespace ConsoleRPG.Items;

public abstract class Weapon : Item
{
    public int damage;
    public override int GetDamage() => damage;
}

public abstract class OneHandedWeapon : Weapon
{
    public override void EquipLeft(Player p)
    {
        p.UnequipLeft();
        p.LeftHand = this;
        p.Inventory.Remove(this);
        p.LogMessage = $"{this.Name} is in left hand";
    }
    public override void EquipRight(Player p)
    {
        p.UnequipRight();
        p.RightHand = this;
        p.Inventory.Remove(this);
        p.LogMessage = $"{this.Name} is in right hand";
    }
    public override void UnEquip(Player p)
    {
       if (p.LeftHand == this) p.LeftHand = null;
       if (p.RightHand == this) p.RightHand = null;
       p.Inventory.Add(this);
       p.LogMessage = $"Unequiped {Name}";
    }

}

public abstract class TwoHandedWeapon : Weapon
{
    private void EquipBoth(Player p)
    {
        p.UnequipLeft();
        p.UnequipRight();
        p.LeftHand = this;
        p.RightHand = this;
        p.Inventory.Remove(this);
        p.LogMessage = $"{Name} is in 2 hands now";
    }
    public override void EquipLeft(Player p) => EquipBoth(p);
    public override void EquipRight(Player p) => EquipBoth(p);
    public override void UnEquip(Player p)
    {
        p.LeftHand = null;
        p.RightHand = null;
        p.Inventory.Add(this);
        p.LogMessage = $"Unequiped {Name}.";
    }
   
} 
public class Sword : OneHandedWeapon { public override char Symbol => '†'; public override string Name => "Sword"; public Sword() { damage = 6; }}
public class Axe : OneHandedWeapon { public override char Symbol => 'P'; public override string Name => "Axe"; public Axe() { damage = 5; } }
public class Greatsword : TwoHandedWeapon { public override char Symbol => 'W'; public override string Name => "Greatsword"; public Greatsword() { damage = 12; } }