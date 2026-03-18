using ConsoleRPG.Items;

namespace ConsoleRPG.World;
public static class DungeonTemplate
{
    public static void ApplyStandardDungeon(Map map)
    {
        new DungeonBuilder()
            .AddStep(new FilledDungeon())
            .AddStep(new StartingExit())
            .AddStep(new CentralRoom(5,5))
            .AddStep(new RandomCorridor(50))
            .AddStep(new ItemsGenerator(5))
            .AddStep(new WeaponsGenerator(2))
            .Build(map);
    }
    public static void ApplyStandardLabyrinth(Map map)
    {
        new DungeonBuilder()
            .AddStep(new FilledDungeon())
            .AddStep(new RandomCorridor(150))
            .AddStep(new ItemsGenerator(5))
            .AddStep(new WeaponsGenerator(2))
            .Build(map);
    }
    public static void ApplyEmptyArena(Map map)
    {
        new DungeonBuilder()
            .AddStep(new EmptyDungeon())
            .Build(map);
    }

    public static void ApplyTreasury(Map map)
    {
        new DungeonBuilder()
        .AddStep(new FilledDungeon())
            .AddStep(new StartingExit())
            .AddStep(new RandomCorridor(50))
            .AddStep(new CurrencyGenerator(10))
            .Build(map);
    }
}