using ConsoleRPG.Entities;
using ConsoleRPG.World;

namespace ConsoleRPG.Items;


public abstract class Currency : Item
{
    public abstract void ApplyToWallet(Player p);
    public override void PickUp(Player p, Cell cell)
    {
        ApplyToWallet(p);
        cell.Items.Remove(this);
    }
}

public class Coin : Currency
{
    public override char Symbol => 'o';
    public override string Name => "Coin";

    public override void ApplyToWallet (Player p)
    {
        p.Coins += 5;
        p.LogMessage = "Picked up 5 coins";
    }
}
public class Gold : Currency
{
    public override char Symbol => 'G';
    public override string Name => "Gold";

    public override void ApplyToWallet (Player p)
    {
        p.Gold += 1;
        p.LogMessage = "Picked up a gold bar";
    }
}
