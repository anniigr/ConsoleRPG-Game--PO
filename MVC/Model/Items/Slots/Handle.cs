using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
using ConsoleRPG.World;
using ConsoleRPG.Log;

namespace ConsoleRPG.Items;

public class Handle : Item, ISlottable, IHasSlots
{
    private readonly SlottedContainerHelper _container;

    public override char Symbol => 'H';
    public override string Name => $"Uchwyt [{_container.SlotCount} slotów]";

    public int SlotCount => _container.SlotCount;
    public IReadOnlyList<ISlottable?> Slots => _container.Slots;
    public override int SoundRange => 3;

    public Handle(int slotCount = 2)
    {
        _container = new SlottedContainerHelper(slotCount);
    }

    // ISlottable 
    public int GetStrengthBonus() => _container.GetTotalStrengthBonus();
    public int GetLuckBonus() => _container.GetTotalLuckBonus();
    public int GetWisdomBonus() => _container.GetTotalWisdomBonus();
    public int GetDamageBonus() => _container.GetTotalDamageBonus();

    // IHasSlots
    public bool TryInsert(ISlottable item, int slotIndex) => _container.TryInsert(item, slotIndex);
    public bool TryRemove(int slotIndex, out ISlottable? removed) => _container.TryRemove(slotIndex, out removed);
    public int GetTotalStrengthBonus() => _container.GetTotalStrengthBonus();
    public int GetTotalLuckBonus() => _container.GetTotalLuckBonus();
    public int GetTotalWisdomBonus() => _container.GetTotalWisdomBonus();
    public int GetTotalDamageBonus() => _container.GetTotalDamageBonus();

    public override void Accept(IAttackVisitor visitor) => visitor.Visit(this);

    public override void PickUp(Player p, Map map)
    {
        p.Inventory.Add(this);
        map.GetCell(p.X, p.Y).Items.Remove(this);
        GameLogger.GetInstance().Log($"Podniósłeś: {Name}. Hałas słyszalny w promieniu {SoundRange} kratek.");
        map.soundManager.Notify(p.X, p.Y, SoundRange, map);
    }

    public string GetSlotsDescription(int indent = 0)
    {
        var sb = new System.Text.StringBuilder();
        string pad = new string(' ', indent);
        sb.AppendLine($"{pad}{Name}");
        for (int i = 0; i < SlotCount; i++)
        {
            if (Slots[i] == null)
                sb.AppendLine($"{pad}  Slot {i + 1}: [pusty]");
            else if (Slots[i] is Handle h)
                sb.Append(h.GetSlotsDescription(indent + 4));
            else
                sb.AppendLine($"{pad}  Slot {i + 1}: {Slots[i]!.Name}");
        }
        return sb.ToString();
    }
}

internal class SlottedContainerHelper : SlottedContainer
{
    public SlottedContainerHelper(int slotCount) : base(slotCount) { }
}