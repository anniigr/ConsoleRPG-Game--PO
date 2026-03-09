using ConsoleRPG.World;
using ConsoleRPG.Entities;
using System.Security.Cryptography.X509Certificates;
namespace ConsoleRPG.Items;


public abstract class Item
{
    public abstract char Symbol {get;}
    public abstract string Name {get;}

    public virtual int GetDamage() {return 0;}
    public virtual void EquipLeft(Player p){p.LogMessage = $"{Name} cannot be in left hand"; }
    public virtual void EquipRight(Player p) { p.LogMessage = $"{Name} cannot be in right hand"; }
    public virtual void UnEquip (Player p){}

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



