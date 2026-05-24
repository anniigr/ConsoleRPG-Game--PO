using ConsoleRPG.Items;
using ConsoleRPG.Entities;
namespace ConsoleRPG.World;

public class Cell{
    public Terrain Terrain {get;set;}
    public List<Item> Items {get;set;} = new List<Item>();
    public Enemy? Enemy {get;set;}
    public Cell (Terrain terrain)
    {
        Terrain = terrain;
        Items = new List<Item>();
    }

    public char GetDrawSymbol()
    {
        if (Enemy != null) return Enemy.Symbol;
        else if (Items.Count() > 0) return Items.Last().Symbol;
        return Terrain.Symbol;
    }
}