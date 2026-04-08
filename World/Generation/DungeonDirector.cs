namespace ConsoleRPG.World;


public class DungeonDirector
{
    private IDungeonBuilder _builder;
    public DungeonDirector(IDungeonBuilder builder)
    {
        _builder = builder;
    }
    public Map CreateStandardDungeon()
    {
        return _builder
            .SetSize(40, 20)
            .AddStep(new FilledDungeon())
            .AddStep(new StartingExit())
            .AddStep(new CentralRoom(5, 5))
            .AddStep(new RandomCorridor(120))
            .AddStep(new ItemsGenerator(5))
            .AddStep(new WeaponsGenerator(2))
            .AddStep(new CurrencyGenerator(2))
            .AddStep(new EnemyGenerator(5))
            .Build();
    }

    public Map CreateStandardLabyrinth()
    {
         return _builder
            .SetSize(40, 20)
            .AddStep(new FilledDungeon())
            .AddStep(new RandomCorridor(150))
            .AddStep(new ItemsGenerator(5))
            .AddStep(new WeaponsGenerator(2))
            .Build();
    }

    public Map CreateEmptyArena()
    {
         return _builder
            .SetSize(40, 20)
            .AddStep(new EmptyDungeon())
            .Build();
    }

    public Map CreateTreasury()
    {
         return _builder
            .SetSize(40, 20)
            .AddStep(new FilledDungeon())
            .AddStep(new StartingExit())
            .AddStep(new RandomCorridor(50))
            .AddStep(new CurrencyGenerator(10))
            .Build();
    }
    public Map CreateWeaponDungeon()
    {
        return _builder
            .SetSize(40, 20)
            .AddStep(new EmptyDungeon())
            .AddStep(new WeaponsGenerator(20))
            .AddStep(new EnemyGenerator(5))
            .Build();
    }

}