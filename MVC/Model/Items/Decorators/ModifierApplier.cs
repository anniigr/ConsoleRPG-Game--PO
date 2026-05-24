using ConsoleRPG.World;

namespace ConsoleRPG.Items;
public static class ModifierApplier {
    private static readonly List<Func<Item,Item>> _availableModifiers = new()
    {
        item =>  new StrongModifier(item),
        item =>  new UnluckyModifier(item)
    };
    public static Item ApplyRandom(Item item)
    {
        var random = Rng.Instance;
        var randomFunc = _availableModifiers[random.Next(_availableModifiers.Count())];
        return randomFunc(item);
    }

}