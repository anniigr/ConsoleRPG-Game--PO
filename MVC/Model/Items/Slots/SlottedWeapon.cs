using ConsoleRPG.Entities;
using ConsoleRPG.Combat;
using ConsoleRPG.World;
using ConsoleRPG.Log;

namespace ConsoleRPG.Items;

public class SlottedSword : LightWeapon, IHasSlots
{
    private readonly SlottedContainerHelper _container;

    public override char Symbol => '†';
    public override string Name => $"Sword with [{_container.SlotCount} slots] (+{damage} dmg)";

    public int SlotCount => _container.SlotCount;
    public IReadOnlyList<ISlottable?> Slots => _container.Slots;

    public SlottedSword(int damage = 2, int slotCount = 2)
    {
        this.damage = damage;
        _container = new SlottedContainerHelper(slotCount);
    }

    public bool TryInsert(ISlottable item, int slotIndex) => _container.TryInsert(item, slotIndex);
    public bool TryRemove(int slotIndex, out ISlottable? removed) => _container.TryRemove(slotIndex, out removed);

    public int GetTotalStrengthBonus() => _container.GetTotalStrengthBonus();
    public int GetTotalLuckBonus() => _container.GetTotalLuckBonus();
    public int GetTotalWisdomBonus() => _container.GetTotalWisdomBonus();
    public int GetTotalDamageBonus() => _container.GetTotalDamageBonus();

    public override int GetDamage() => damage + GetTotalDamageBonus();

    public override void EquipLeft(Player p, Item self)
    {
        base.EquipLeft(p, self);
        ApplySlotBonuses(p, +1);
        LogBonuses();
    }

    public override void EquipRight(Player p, Item self)
    {
        base.EquipRight(p, self);
        ApplySlotBonuses(p, +1);
        LogBonuses();
    }

    public override void UnEquip(Player p, Item self)
    {
        ApplySlotBonuses(p, -1);
        base.UnEquip(p, self);
        GameLogger.GetInstance().Log($"Put off {Name}, bonuses cancelled.");
    }

    private void ApplySlotBonuses(Player p, int sign)
    {
        p.Strength += sign * GetTotalStrengthBonus();
        p.Luck += sign * GetTotalLuckBonus();
        p.Wisdom += sign * GetTotalWisdomBonus();
    }

    private void LogBonuses()
    {
        int str = GetTotalStrengthBonus();
        int luck = GetTotalLuckBonus();
        int wis = GetTotalWisdomBonus();
        GameLogger.GetInstance().Log($"{Name}: +{str} siła, +{luck} szczęście, +{wis} mądrość ze slotów.");
    }

    public string GetSlotsDescription()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"{Name}:");
        for (int i = 0; i < SlotCount; i++)
        {
            if (Slots[i] == null)
                sb.AppendLine($"  Slot {i + 1}: [pusty]");
            else if (Slots[i] is Handle h)
                sb.Append(h.GetSlotsDescription(4));
            else
                sb.AppendLine($"  Slot {i + 1}: {Slots[i]!.Name}");
        }
        return sb.ToString();
    }
}