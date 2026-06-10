using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
using ConsoleRPG.World;
using ConsoleRPG.Log;

namespace ConsoleRPG.Items;
public abstract class PassiveStone : Item, ISlottable
{
    public virtual int GetStrengthBonus() => 0;
    public virtual int GetLuckBonus() => 0;
    public virtual int GetWisdomBonus() => 0;
    public virtual int GetDamageBonus() => 0;
    public override int SoundRange => 2;

    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);

    public override void PickUp(Player p, Map map)
    {
        p.Inventory.Add(this);
        map.GetCell(p.X, p.Y).Items.Remove(this);
        GameLogger.GetInstance().Log($"Podniósłeś: {Name}. Hałas słyszalny w promieniu {SoundRange} kratek.");
        if (SoundRange > 0)
        {
            map.soundManager.Notify(p.X, p.Y, SoundRange, map);
        }
    }
}

public class StrengthStone : PassiveStone
{
    public override char Symbol => '◆';
    public override string Name => "Strength stone (+2 to Strength)";
    public override int GetStrengthBonus() => 2;
    
}

public class LuckStone : PassiveStone
{
    public override char Symbol => '◇';
    public override string Name => "Luck Stone (+2 to luck)";
    public override int GetLuckBonus() => 2;
}

public class WisdomStone : PassiveStone
{
    public override char Symbol => '◈';
    public override string Name => "Wisdom stone (+2 to wisdom)";
    public override int GetWisdomBonus() => 2;
}