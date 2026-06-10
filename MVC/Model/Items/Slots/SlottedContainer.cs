namespace ConsoleRPG.Items;

public abstract class SlottedContainer : IHasSlots
{
    private readonly ISlottable?[] _slots;

    public int SlotCount => _slots.Length;
    public IReadOnlyList<ISlottable?> Slots => _slots;

    protected SlottedContainer(int slotCount)
    {
        _slots = new ISlottable?[slotCount];
    }

    public bool TryInsert(ISlottable item, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return false;
        if (_slots[slotIndex] != null) return false;
        _slots[slotIndex] = item;
        return true;
    }

    public bool TryRemove(int slotIndex, out ISlottable? removed)
    {
        removed = null;
        if (slotIndex < 0 || slotIndex >= _slots.Length) return false;
        removed = _slots[slotIndex];
        _slots[slotIndex] = null;
        return removed != null;
    }

    public int GetTotalStrengthBonus()
    {
        int total = 0;
        foreach (var s in _slots)
            if (s != null) total += s.GetStrengthBonus();
        return total;
    }

    public int GetTotalLuckBonus()
    {
        int total = 0;
        foreach (var s in _slots)
            if (s != null) total += s.GetLuckBonus();
        return total;
    }

    public int GetTotalWisdomBonus()
    {
        int total = 0;
        foreach (var s in _slots)
            if (s != null) total += s.GetWisdomBonus();
        return total;
    }

    public int GetTotalDamageBonus()
    {
        int total = 0;
        foreach (var s in _slots)
            if (s != null) total += s.GetDamageBonus();
        return total;
    }
}