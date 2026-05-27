using System.Security.Cryptography.X509Certificates;
using ConsoleRPG.Items;
using ConsoleRPG.World;
using ConsoleRPG.Combat;
using ConsoleRPG.Log;
using ConsoleRPG.Entities.Observers;
namespace ConsoleRPG.Entities;

// Reciever
public class Player : Entity,IEventListenerSound
{
    private int _baseStrength = 10; 
public int Strength 
{
    get 
    {
        int total = _baseStrength;
        if (RightHand is Item rItem) total += GetItemStrengthBonus(rItem);
        if (LeftHand is Item lItem && LeftHand != RightHand) total += GetItemStrengthBonus(lItem);
        return total;
    }
    set { _baseStrength = value; } 
    }

private int _baseLuck = 10; 
public int Luck 
{
    get 
    {
        int total = _baseLuck;
        if (RightHand is Item rItem) total += GetItemLuckBonus(rItem);
        if (LeftHand is Item lItem && LeftHand != RightHand) total += GetItemLuckBonus(lItem);
        return total;
    }
    set { _baseLuck = value; }
}
    public int Dexterity { get; set; } = 10; 
    public int Aggression { get; set; } = 5;
    public int Wisdom { get; set; } = 5;
    public int SyncedTotalDamage { get; set; } = -1;

    public int Coins {get; set;}
    public int Gold {get; set;}
    public int Id { get; set; }

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
        if (SyncedTotalDamage != -1) return SyncedTotalDamage;
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
            item.PickUp(this, map); 
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
    public void SoundProduced(int dist, int sourceX, int sourceY)
    {
        GameLogger.GetInstance().LogToSpecificPlayer(this.Id, $"[SŁUCH] {this.Name} słyszy hałas z odległości {dist} pól!");
    }
    private int GetItemStrengthBonus(Item item)
{
    if (item.Name.Contains("(Strong)")) return 5;
    return 0;
}

private int GetItemLuckBonus(Item item)
{
    if (item.Name.Contains("(Unlucky)")) return -5; 
    return 0;
}
}