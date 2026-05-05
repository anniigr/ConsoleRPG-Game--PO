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
    private int _roomNumber;
    public RandomRoom(int number)
    {
        _roomNumber = number;
    }

    public void Apply (Map map)
    {
        Random rand =Rng.Instance;
        int startX = rand.Next(1,map.Width - 1);
        int startY = rand.Next(1,map.Height - 1);

        int roomWidth = rand.Next(map.Width/2);
        int roomHeight = rand.Next(1,map.Height/2);

        for(int i =0; i < _roomNumber; i++){
            for (int x = startX; x < startX + roomWidth; x++)
                for (int y = startY; y < startY + roomHeight; y++)
                    if (map.IsInsideInnerArea(x,y))
                        map.SetCell(x,y, new Cell(new Floor()));
        }
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
    private IEnumerable<Func<Item>> FuncPossibleItems;
    private int itemsCount;
    
    public ItemsGenerator(IEnumerable<Func<Item>> FuncItems, int count)
    {
        this.FuncPossibleItems = FuncItems;
        this.itemsCount = count;
    }

    public void Apply(Map map)
    {
        Random rand = Rng.Instance;
        int placed = 0;
        int attempts = 0;

        List<Func<Item>> listOfFunc  =  FuncPossibleItems.ToList();

        while (placed < itemsCount && attempts < 1000)
        {
            int startX = rand.Next(1,map.Width - 1);
            int startY = rand.Next(1,map.Height - 1);
            Cell cell = map.GetCell(startX,startY);

            if(map.IsInsideInnerArea(startX,startY) && cell.Terrain.IsPassable())
            {
                Func<Item> CreateItem = listOfFunc[rand.Next(listOfFunc.Count)];
                Item item = CreateItem();
                cell.Items.Add(item);
                placed ++;
            }
            attempts ++;
        }
    }   
    public string GetDescription() => $"{itemsCount} random items created" ;
}

public class EnemyGenerator : IDungeonStep 
{
    private int _enemyCount;
    private IEnumerable<SpeciesSpawnDefinition> _speciesDefinitions;

    public EnemyGenerator(IEnumerable<SpeciesSpawnDefinition> definitions, int enemyCount)
    {
        _speciesDefinitions = definitions;
        _enemyCount = enemyCount;
    }

    public void Apply(Map map)
    {
        Random rand = Rng.Instance;
        var definitionsList = _speciesDefinitions.ToList();
        int totalPlaced = 0; 

        foreach (var def in definitionsList)
        {
            int spawnedForThisSpecies = 0;
            int attempts = 0;
            
            while (spawnedForThisSpecies < def.MinCount && attempts < 1000)
            {
                if (TrySpawnEnemy(map, def.Factory))
                {
                    spawnedForThisSpecies++;
                    totalPlaced++; 
                }
                attempts++;
            }
        }

        int fillAttempts = 0;
        while (totalPlaced < _enemyCount && fillAttempts < 2000)
        {
            var randomDef = definitionsList[rand.Next(definitionsList.Count)];
            
            if (TrySpawnEnemy(map, randomDef.Factory))
            {
                totalPlaced++;
            }
            fillAttempts++;
        }
    }

    private bool TrySpawnEnemy(Map map, Func<int, int, Enemy> factory)
    {
        Random rand = Rng.Instance;
        int x = rand.Next(1, map.Width - 1);
        int y = rand.Next(1, map.Height - 1);
        Cell cell = map.GetCell(x, y);

        if (cell.Terrain.IsPassable() && cell.Enemy == null)
        {
            var enemy = factory(x, y);
            cell.Enemy = enemy;
            enemy.MoveRandomly(map);
            enemy.enemiesManagerSound = map.soundManager;
            map.soundManager.Subscribe(enemy);
            return true;
        }
        return false;
    }

    public string GetDescription() => $"Enemies created with guaranteed species counts.";
}
public class ArtifactGenerator : IDungeonStep
{
    private readonly Item _artifact;

    public ArtifactGenerator(Item artifact)
    {
        _artifact = artifact;
    }

    public void Apply(Map map)
    {
        Random rand = Rng.Instance;
        int attempts = 0;
        
        while (attempts < 1000)
        {
            int startX = rand.Next(1, map.Width - 1);
            int startY = rand.Next(1, map.Height - 1);
            Cell cell = map.GetCell(startX, startY);
            
            if (map.IsInsideInnerArea(startX, startY) && cell.Terrain.IsPassable())
            {
                cell.Items.Add(_artifact);
                break; 
            }
            attempts++;
        }
    }

    public string GetDescription() => $"Artifact '{_artifact.Name}' placed.";
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

public class SpeciesSpawnDefinition
{
    public Func<int, int, Enemy> Factory { get; }
    public int MinCount { get; }

    public SpeciesSpawnDefinition(Func<int, int, Enemy> factory, int minCount)
    {
        Factory = factory;
        MinCount = minCount;
    }
}