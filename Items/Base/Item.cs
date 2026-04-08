using ConsoleRPG.World;
using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
using System.Security.Cryptography.X509Certificates;
namespace ConsoleRPG.Items;

public abstract class Item
{
    public abstract char Symbol {get;}
    public abstract string Name {get;}

    public virtual int GetDamage() {return 0;}
    public virtual void EquipLeft(Player p)=> EquipLeft(p, this);
    public virtual void EquipRight(Player p) => EquipRight(p, this);
    public virtual void UnEquip(Player p) => UnEquip(p, this);

    public virtual void EquipLeft(Player p, Item self)
    {
        p.UnequipLeft(); 
        p.LeftHand = self;
        p.Inventory.Remove(self);
        p.LogMessage = $"{Name} is in left hand.";
    }

    public virtual void EquipRight(Player p, Item self)
    {
        p.UnequipRight(); 
        p.RightHand = self;
        p.Inventory.Remove(self);
        p.LogMessage = $"{Name} is in right hand.";
    }

    public virtual void UnEquip(Player p, Item self)
    {
        if (p.LeftHand == self) p.LeftHand = null;
        if (p.RightHand == self) p.RightHand = null;
        
        if (!p.Inventory.Contains(self))
            p.Inventory.Add(self);
            
        p.LogMessage = $"Unequipped {Name}.";
    }

    public virtual void Accept(IAttackVisitor visitor) => visitor.Visit(this);
    public virtual void Drop(Player p, Cell cell)
    {
        if(p.Inventory.Contains(this))
        {
            p.Inventory.Remove(this);
            cell.Items.Add(this);
            
            p.LogMessage = $"Thrown: {this.Name}.";
        }
    }

    public virtual void PickUp(Player p, Cell cell)
    {
        p.Inventory.Add(this);
        cell.Items.Remove(this);
        p.LogMessage = $"Picked up: {Name}.";
    }

}



