using ConsoleRPG.World;
using ConsoleRPG.Entities;
using ConsoleRPG.Log;
using ConsoleRPG.Combat;
using System.Security.Cryptography.X509Certificates;
namespace ConsoleRPG.Items;

public abstract class Item
{
    protected char _symbol;
    protected string _name;

    public virtual char Symbol => _symbol;
    public virtual string Name => _name;

    public virtual int GetDamage() {return 0;}
    public virtual void EquipLeft(Player p)=> EquipLeft(p, this);
    public virtual void EquipRight(Player p) => EquipRight(p, this);
    public virtual void UnEquip(Player p) => UnEquip(p, this);

    public virtual void EquipLeft(Player p, Item self)
    {
        p.UnequipLeft(); 
        p.LeftHand = self;
        p.Inventory.Remove(self);
        GameLogger.GetInstance().Log($"{Name} is in left hand.");
    }

    public virtual void EquipRight(Player p, Item self)
    {
        p.UnequipRight(); 
        p.RightHand = self;
        p.Inventory.Remove(self);
        GameLogger.GetInstance().Log($"{Name} is in right hand.");
    }

    public virtual void UnEquip(Player p, Item self)
    {
        if (p.LeftHand == self) p.LeftHand = null;
        if (p.RightHand == self) p.RightHand = null;
        
        if (!p.Inventory.Contains(self))
            p.Inventory.Add(self);
            
       GameLogger.GetInstance().Log($"Unequipped {Name}.");
    }

    public virtual void Accept(IAttackVisitor visitor) => visitor.Visit(this);
    public virtual void Drop(Player p, Cell cell)
    {
        if(p.Inventory.Contains(this))
        {
            p.Inventory.Remove(this);
            cell.Items.Add(this);
            
            GameLogger.GetInstance().Log($"Thrown: {this.Name}.");
        }
    }

    public virtual void PickUp(Player p, Map map)
    {
        p.Inventory.Add(this);
        Cell cell = map.GetCell(p.X,p.Y);
        cell.Items.Remove(this);
        GameLogger.GetInstance().Log($"Picked up: {Name}.");
    }

}



