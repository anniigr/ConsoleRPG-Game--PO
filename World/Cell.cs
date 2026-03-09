using ConsoleRPG.Items;
namespace ConsoleRPG.World;

public class Cell{
    public Terrain Terrain {get;set;}
    public List<Item> Items {get;set;} = new List<Item>();
    public Cell (Terrain terrain)
    {
        Terrain = terrain;
        Items = new List<Item>();
    }

    public char GetDrawSymbol()
    {
        if (Items.Count() > 0) return Items.Last().Symbol;
        return Terrain.Symbol;
    }
}