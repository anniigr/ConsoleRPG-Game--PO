namespace ConsoleRPG.Items;


public interface IHasSlots
{
    int SlotCount { get; }
    IReadOnlyList<ISlottable?> Slots { get; }
    bool TryInsert(ISlottable item, int slotIndex);
    bool TryRemove(int slotIndex, out ISlottable? removed);
    int GetTotalStrengthBonus();
    int GetTotalLuckBonus();
    int GetTotalWisdomBonus();
    int GetTotalDamageBonus();
}