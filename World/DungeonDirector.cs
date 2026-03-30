namespace ConsoleRPG.World;


public class DungeonDirector
{
    public Map CreateStandardDungeon()
    {
        return new DungeonBuilder()
            .SetSize(40, 20)
            .AddStep(new FilledDungeon())
            .AddStep(new StartingExit())
            .AddStep(new CentralRoom(5, 5))
            .AddStep(new RandomCorridor(50))
            .AddStep(new ItemsGenerator(5))
            .AddStep(new WeaponsGenerator(2))
            .Build();
    }

    public Map CreateStandardLabyrinth()
    {
        return new DungeonBuilder()
            .SetSize(40, 20)
            .AddStep(new FilledDungeon())
            .AddStep(new RandomCorridor(150))
            .AddStep(new ItemsGenerator(5))
            .AddStep(new WeaponsGenerator(2))
            .Build();
    }

    public Map CreateEmptyArena()
    {
        return new DungeonBuilder()
            .SetSize(40, 20)
            .AddStep(new EmptyDungeon())
            .Build();
    }

    public Map CreateTreasury()
    {
        return new DungeonBuilder()
            .SetSize(40, 20)
            .AddStep(new FilledDungeon())
            .AddStep(new StartingExit())
            .AddStep(new RandomCorridor(50))
            .AddStep(new CurrencyGenerator(10))
            .Build();
    }
}