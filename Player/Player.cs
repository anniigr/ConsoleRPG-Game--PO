using System.Security.Cryptography.X509Certificates;
using ConsoleRPG.Engine;
using ConsoleRPG.Items;
using ConsoleRPG.World;
using ConsoleRPG.Combat;
namespace ConsoleRPG.Entities;

// Reciever
public class Player : Entity
{
    
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10; 
    public int Luck { get; set; } = 5;
    public int Aggression { get; set; } = 5;
    public int Wisdom { get; set; } = 5;

    public int Coins {get; set;}
    public int Gold {get; set;}

    public Item? LeftHand {get; set;} = null;
    public Item? RightHand {get;set;} = null;

    public List<Item> Inventory {get;set;} = new List<Item>();


    public Player(int startX, int startY) 
    : base ("Hero", '¶', startX, startY, 100){}


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
        if (target == null ) return;
        if (target.Enemy != null)
        {
            AttackEnemy(target.Enemy, new NormalAttack(this));
            if (target.Enemy.Health <= 0) target.Enemy = null;
            return;
        }
        if (target.Terrain.IsPassable())
        {
            X = newX;
            Y = newY;
            GameLogger.GetInstance().Log("The player is moved");
        }
        else
        {
             GameLogger.GetInstance().Log("The wall is blocking way!");
        }
        if (Health <= 0)
        {
             GameLogger.GetInstance().Log("GAME OVER. You died in the dungeon.");
        }
    }
    public void AttackEnemy(Enemy enemy, IAttackVisitor attackType)
    {
        int totalDamage = 0;
        int totalDefense = 0;

        void ApplyHand(Item? hand) {
            if (hand == null) return;
            hand.Accept(attackType);
            totalDamage += attackType.ResultDamage;
            totalDefense += attackType.ResultDefense;
        }

        ApplyHand(RightHand);
        if (LeftHand != RightHand) ApplyHand(LeftHand);
        enemy.TakeDamage(totalDamage);
        
        int damageFromEnemy = Math.Max(0, enemy.AttackValue - totalDefense);
        Health -= damageFromEnemy;

        GameLogger.GetInstance().Log($"You used {attackType.GetType().Name} on {enemy.Name}. Out: {totalDamage} dmg, Defended: {totalDefense}. Taken: {damageFromEnemy} dmg.");
        if (enemy.Health <= 0)  GameLogger.GetInstance().Log($"{enemy.Name} is killed.");
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
            GameLogger.GetInstance().Log("Nothing to pick up");
        }
    }

    public void DropItem(Item item, Map map)
    {
        item.Drop(this,map.GetCell(X,Y));
    }
}