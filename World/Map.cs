using ConsoleRPG.Items;
namespace ConsoleRPG.World;

public class Map
{
    public int Width = 40;
    public int Height = 20;
    private Cell[,] grid;

    public Map()
    {
        grid = new Cell[Width, Height];
        InitializeMap();
    }
    public void InitializeMap()
    {
        for (int i =0; i < Width; i++)
        {
            for (int j =0; j < Height; j++)
            {
               if ((i == 0 && j != 0) || i == Width - 1 || j == Height - 1 || (j == 0 && i != 0))
                {
                    grid[i,j] = new Cell(new Wall());
                }
                else grid[i,j] = new Cell(new Floor());
            }
        }
        grid[0, 1] = new Cell(new Floor());
        grid[1, 0] = new Cell(new Floor());

        //wall
        for(int i = 10; i < 30; i++)
        {
            grid[i,10] = new Cell(new Wall());
        }

        GetCell(2, 2).Items.Add(new Sword());
        GetCell(3, 2).Items.Add(new Axe());
        GetCell(4, 2).Items.Add(new Greatsword()); 
            
        GetCell(5, 5).Items.Add(new Mug());
        GetCell(6, 5).Items.Add(new Bone());
        GetCell(7, 5).Items.Add(new Rope());

        GetCell(10, 2).Items.Add(new Coin());
        GetCell(11, 2).Items.Add(new Coin());
        GetCell(12, 2).Items.Add(new Gold());
    }

    public Cell GetCell(int i, int j)
    {
        if(i >= 0 && i < Width && j >= 0 && j < Height)
        return grid[i,j];

        return null!;
    }
}