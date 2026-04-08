using System.Security.Cryptography.X509Certificates;
using ConsoleRPG.Entities;
using ConsoleRPG.Items;
namespace ConsoleRPG.World;

public static class Rng
{
    public static Random Instance = new Random();
}
public interface IDungeonStep
{
    void Apply(Map map);
    string GetDescription();
}
public class EmptyDungeon : IDungeonStep
{
    public void Apply (Map map)
    {
        for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
                map.SetCell(x,y, new Cell(new Floor()));
    }
    public string GetDescription() => "Empty dungeon created" ;

}
public class FilledDungeon : IDungeonStep
{
    public void Apply (Map map)
    {
        for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
                map.SetCell(x,y, new Cell(new Wall()));

        map.SetCell(0,0, new Cell(new Floor()));
        map.SetCell(1,0, new Cell(new Floor()));
        map.SetCell(0,1, new Cell(new Floor()));
    }
    public string GetDescription() => "Filled dungeon created" ;
}

public class CentralRoom : IDungeonStep
{
    private int roomWidth;
    private int roomHeight;
    public CentralRoom(int width, int height)
    {
        this.roomWidth = width;
        this.roomHeight = height;
    }
    public void Apply (Map map)
    {
        int startX = (map.Width - roomWidth)/2;
        int startY = (map.Height - roomHeight)/2;

        for (int x = startX; x < startX + roomWidth; x++)
            for (int y = startY; y < startY + roomHeight; y++)
                if(map.IsInsideInnerArea(x,y))
                    map.SetCell(x,y, new Cell(new Floor()));
    }
    public string GetDescription() => $"Central room {roomWidth}*{roomHeight} created" ;
}

public class RandomRoom : IDungeonStep
{

    public void Apply (Map map)
    {
        Random rand =Rng.Instance;
        int startX = rand.Next(1,map.Width - 1);
        int startY = rand.Next(1,map.Height - 1);

        int roomWidth = rand.Next(1,3);
        int roomHeight = rand.Next(1,3);

        for (int x = startX; x < startX + roomWidth; x++)
            for (int y = startY; y < startY + roomHeight; y++)
                if (map.IsInsideInnerArea(x,y))
                    map.SetCell(x,y, new Cell(new Floor()));
    }
    public string GetDescription() => "Random room created" ;
}

public class RandomCorridor : IDungeonStep
{
    private int corridorCount;
    public RandomCorridor(int corridorCount)
    {
        this.corridorCount = corridorCount;
    }
    public void Apply(Map map)
    {
        Random rand =Rng.Instance;
        for (int i = 0; i < corridorCount; i++)
        {
            int startX = rand.Next(1, map.Width - 1);
            int startY = rand.Next(1, map.Height - 1);
            int length = rand.Next(2, 10);
            bool horizontal = rand.Next(2) == 0;

            for (int j = 0; j < length; j++)
            {
                int currX = horizontal ? startX + j : startX;
                int currY = horizontal ? startY : startY + j;

                if (map.IsInsideInnerArea(currX, currY))
                {
                    map.SetCell(currX, currY, new Cell(new Floor()));
                }
            }
        }
    }
    public string GetDescription() => $"{corridorCount} random corridors created" ;
}

public class ItemsGenerator : IDungeonStep {
    int itemsCount;
    public ItemsGenerator(int itemsCount)
    {
        this.itemsCount = itemsCount;
    }
    Item[] possibleItems =
    {
        new Bone(),
        new Mug(),
        new Rope()
    };

    public void Apply(Map map)
    {
        Random rand =Rng.Instance;
        int placed = 0;
        int attempts = 0;
        while (placed < itemsCount && attempts < 1000){
            int startX = rand.Next(1,map.Width - 1);
            int startY = rand.Next(1,map.Height - 1);
            Cell cell = map.GetCell(startX,startY);

            if(map.IsInsideInnerArea(startX,startY) && cell.Terrain.IsPassable())
            {
                Item item = possibleItems[rand.Next(possibleItems.Length)];
                cell.Items.Add(item);
                placed ++;
            }
            attempts ++;
        }
    }   
    public string GetDescription() => $"{itemsCount} random items created" ;
}
public class WeaponsGenerator : IDungeonStep {
    int weaponsCount;
    public WeaponsGenerator(int weaponsCount)
    {
        this.weaponsCount = weaponsCount;
    }
    Func<Weapon>[] weaponFactories = { () => new Axe(), () => new MagicStaff(), () => new Sword() };

    public void Apply(Map map)
    {
        Random rand = Rng.Instance;

        int placed = 0, attempts = 0;
        while (placed < weaponsCount && attempts < 1000){
            int startX = rand.Next(1,map.Width - 1);
            int startY = rand.Next(1,map.Height - 1);
            Cell cell = map.GetCell(startX,startY);

            attempts++;
            if(cell.Terrain.IsPassable())
            {
                Item item = weaponFactories[rand.Next(weaponFactories.Length)](); 
                int effectsLimit =3;
                for (int i = 0; i < effectsLimit; i++) {
                    if (rand.Next(100) < 50) {
                        item = ModifierApplier.ApplyRandom(item);
                    }
                }
                cell.Items.Add(item);
                placed ++;
            }
        }
        
    }
    public string GetDescription() => $"{weaponsCount} random weapons created" ;
}
public class CurrencyGenerator : IDungeonStep {
    int coinCount;
    public CurrencyGenerator(int coinCount)
    {
        this.coinCount = coinCount;
    }
    Currency[] possibleCurrency =
    {
        new Coin(),
        new Gold()
    };

    public void Apply(Map map)
    {
        Random rand =Rng.Instance;
        int placed = 0, attempts = 0;
        while (placed < coinCount && attempts < 1000){
            int startX = rand.Next(1,map.Width - 1);
            int startY = rand.Next(1,map.Height - 1);
            Cell cell = map.GetCell(startX,startY);
            attempts ++;
            if(cell.Terrain.IsPassable())
            {
                Item item = possibleCurrency[rand.Next(possibleCurrency.Length)];
                cell.Items.Add(item);
                placed ++;
            }
        }   
    }
    public string GetDescription() => $"{coinCount} random currency created" ;
}

public class EnemyGenerator : IDungeonStep {
    int enemyCount;
    public EnemyGenerator(int enemyCount)
    {
        this.enemyCount = enemyCount;
    }
    private Func<Enemy>[] _enemies =
    {
        () => new Enemy("Goblin",'g',30,12,2),
        () => new Enemy("Skeleton",'s',45,15,3),
        () => new Enemy("Zombi",'z',60,16,6)
    };

    public void Apply(Map map)
    {
        Random rand =Rng.Instance;
        int placed = 0, attempts = 0;
        while (placed < enemyCount && attempts < 1000){
            int startX = rand.Next(1,map.Width - 1);
            int startY = rand.Next(1,map.Height - 1);
            Cell cell = map.GetCell(startX,startY);
            attempts ++;
            if(cell.Terrain.IsPassable())
            {
                Enemy enemy = _enemies[rand.Next(_enemies.Length)]();
                cell.Enemy = enemy;
                placed ++;
            }
        }   
    }
    public string GetDescription() => $"{enemyCount} random enemies created" ;
}

public class StartingExit : IDungeonStep
{
    public void Apply(Map map)
    {
        for (int x = 0; x < 5; x++)
        {
            map.SetCell(x, 0, new Cell(new Floor()));
        }
        for (int y = 0; y < 5; y++)
        {
            map.SetCell(4, y, new Cell(new Floor()));
        }
    }
    public string GetDescription() => $"Start door created" ;
}
