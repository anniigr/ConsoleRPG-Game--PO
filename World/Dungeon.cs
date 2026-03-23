using ConsoleRPG.Items;
namespace ConsoleRPG.World;

public static class Rng
{
    public static Random Instance = new Random();
}
public interface IDungeonStep
{
    void Apply(Map map);
}
public class EmptyDungeon : IDungeonStep
{
    public void Apply (Map map)
    {
        for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
                map.SetCell(x,y, new Cell(new Floor()));
    }
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
            int length = rand.Next(2, map.Height);
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
        map.Features.HasItem = true;
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
}
public class WeaponsGenerator : IDungeonStep {
    int weaponsCount;
    public WeaponsGenerator(int weaponsCount)
    {
        this.weaponsCount = weaponsCount;
    }
    Weapon[] possibleWeapons =
    {
        new Axe(),
        new Greatsword(),
        new Sword()
    };

    public void Apply(Map map)
    {
        Random rand = Rng.Instance;
        map.Features.HasWeapon = true;

        int placed = 0, attempts = 0;
        while (placed < weaponsCount && attempts < 1000){
            int startX = rand.Next(1,map.Width - 1);
            int startY = rand.Next(1,map.Height - 1);
            Cell cell = map.GetCell(startX,startY);

            attempts++;
            if(cell.Terrain.IsPassable())
            {
                Item item = possibleWeapons[rand.Next(possibleWeapons.Length)];
                cell.Items.Add(item);
                placed ++;
            }
        }
        
    }
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
        map.Features.HasCurrency = true;
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
}

public class DungeonBuilder
{
    List<IDungeonStep> steps;
    public DungeonBuilder()
    {
        steps = new List<IDungeonStep>();
    }
    public DungeonBuilder AddStep(IDungeonStep step)
    {
        steps.Add(step);
        return this;
    }
    public void Build(Map map)
    {
        if (steps.Count == 0)
        throw new InvalidOperationException("Dungeon must have at least one step.");

        if (!(steps[0] is EmptyDungeon || steps[0] is FilledDungeon))
        throw new InvalidOperationException("First step must be EmptyDungeon or FilledDungeon.");


        foreach (var step in steps)
        {
            step.Apply(map);
        }
    }
} 
