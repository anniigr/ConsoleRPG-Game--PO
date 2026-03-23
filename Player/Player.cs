using System.Security.Cryptography.X509Certificates;
using ConsoleRPG.Items;
using ConsoleRPG.World;
namespace ConsoleRPG.Entities;

public class Player
{
    public int X {get; private set;}
    public int Y {get; private set;}

    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Health { get; set; } = 100;        
    public int Luck { get; set; } = 5;
    public int Aggression { get; set; } = 5;
    public int Wisdom { get; set; } = 5;

    public int Coins {get; set;}
    public int Gold {get; set;}

    public Item? LeftHand {get; set;} = null;
    public Item? RightHand {get;set;} = null;

    public List<Item> Inventory {get;set;} = new List<Item>();

    public string LogMessage {get; set; } = "The game is started";

    public Player(int startX, int startY)
    {
        X = startX; 
        Y = startY;
    }

    public void UnequipLeft()
    {
        if(LeftHand != null) {LeftHand.UnEquip(this);}
    }

    public void UnequipRight()
    {
        if(RightHand != null) {RightHand.UnEquip(this); }
    }
    public int GetTotalDamage()
    {
        int dmg = 0;
        if (LeftHand != null) dmg += LeftHand.GetDamage();

        if (RightHand != null && RightHand != LeftHand) dmg += RightHand.GetDamage();
        return dmg;
    }
    public void Move(int dx, int dy, Map map)
        {
        int newX = X + dx;
        int newY = Y + dy;

        Cell target = map.GetCell(newX, newY);
        if (target != null && target.Terrain.IsPassable())
        {
            X = newX;
            Y = newY;
            LogMessage = "The player is moved";
        }
        else
        {
            LogMessage = "The wall is blocking way!";
        }
    }
    public void PickUp(Map map)
    {
        Cell current = map.GetCell(X, Y);
        if (current.Items.Count > 0)
        {
            Item item = current.Items[current.Items.Count - 1];
            item.PickUp(this, current); 
        }
        else
        {
            LogMessage = "Nothing to pick up";
        }
    }

    public void DropItem(Item item, Map map)
    {
        item.Drop(this,map.GetCell(X,Y));
    }
}