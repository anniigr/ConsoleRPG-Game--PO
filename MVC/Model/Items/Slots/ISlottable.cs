namespace ConsoleRPG.Items;

public interface ISlottable
{
    string Name { get; }
    int GetStrengthBonus();
    int GetLuckBonus();
    int GetWisdomBonus();
    int GetDamageBonus();
}