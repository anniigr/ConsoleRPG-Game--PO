using ConsoleRPG.Items;
namespace ConsoleRPG.World;

public class Map
{
    public int Width = 40;
    public int Height = 20;
    private Cell[,] grid;
    public MapFeatures Features = new MapFeatures();

    public Map()
    {
        grid = new Cell[Width, Height];
        InitializeMap();
    }
    public void InitializeMap()
    {
        Features = new MapFeatures();
        DungeonTemplate.ApplyStandardDungeon(this);
    }

    public Cell GetCell(int i, int j)
    {
        if(i >= 0 && i < Width && j >= 0 && j < Height)
        return grid[i,j];

        return null!;
    }

    public void SetCell(int x, int y, Cell cell)
    {
        grid[x,y] = cell;
    }

    public bool IsInsideInnerArea(int x, int y)
    {
        return x > 0 && x < Width - 1 && y > 0 && y < Height - 1;
    }
}