using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using ConsoleRPG.World;

namespace ConsoleRPG.Tests;

public class WorldAndInventoryTests
{
    [Fact]
    public void PlayerMove_EntersPassableCell()
    {
        var map = CreateMap(3, 3, new Floor());
        var player = new Player(1, 1);

        player.Move(1, 0, map);

        Assert.Equal((2, 1), (player.X, player.Y));
    }

    [Fact]
    public void PlayerMove_IsBlockedByWall()
    {
        var map = CreateMap(3, 3, new Floor());
        map.SetCell(2, 1, new Cell(new Wall()));
        var player = new Player(1, 1);

        player.Move(1, 0, map);

        Assert.Equal((1, 1), (player.X, player.Y));
    }

    [Fact]
    public void PlayerMove_OutsideMapLeavesPositionUnchanged()
    {
        var map = CreateMap(2, 2, new Floor());
        var player = new Player(0, 0);

        player.Move(-1, 0, map);

        Assert.Equal((0, 0), (player.X, player.Y));
    }

    [Fact]
    public void PickingUpCoin_UpdatesWalletAndRemovesCoinFromCell()
    {
        var map = CreateMap(1, 1, new Floor());
        var player = new Player(0, 0);
        var coin = new Coin();
        map.GetCell(0, 0).Items.Add(coin);

        player.PickUp(map);

        Assert.Equal(5, player.Coins);
        Assert.Empty(map.GetCell(0, 0).Items);
        Assert.DoesNotContain(coin, player.Inventory);
    }

    [Fact]
    public void CellDrawSymbol_PrioritizesEnemyOverItemAndTerrain()
    {
        var cell = new Cell(new Floor());
        cell.Items.Add(new Coin());
        cell.Enemy = new SimpleEnemy("Test enemy", 'e', 0, 0, 10, 1, 0);

        Assert.Equal('e', cell.GetDrawSymbol());
    }

    private static Map CreateMap(int width, int height, Terrain terrain)
    {
        var map = new Map(width, height);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                map.SetCell(x, y, new Cell(terrain));
            }
        }

        return map;
    }
}
