using ConsoleRPG.Items;
namespace ConsoleRPG.World;

public class Map
{
    public int Width;
    public int Height;
    private Cell[,] grid;
    public MapFeatures Features = new MapFeatures();

    public Map(int width = 40, int height = 20)
    {
        Width = width;
        Height = height;
        grid = new Cell[Width, Height];
        Features = new MapFeatures();
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