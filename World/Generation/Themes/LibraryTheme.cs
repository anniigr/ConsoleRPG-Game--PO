// ===== ./World/Generation/Themes/LibraryThemeFactory.cs =====
using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using System.Collections.Generic;
using System;

namespace ConsoleRPG.World;

public class LibraryThemeFactory : IDungeonThemeFactory
{
    public string GetGreetingMessage() => "Zapach starych książek wypełnia powietrze. Czuć magię.";
    public (int width, int height) GetSize() => (40, 20);

    public Item CreateArtifact() => new BlackWand();

    public IEnumerable<Func<Item>> CreateItemPool()
    {
        yield return () => new Book();
        yield return () => new Mug();
        yield return () => new Coin();
        yield return () => new Sword();
    }

    public IEnumerable<Func<int, int, Enemy>> CreateEnemyPool()
    {
        yield return (x, y) => new Enemy("Mage", 'M', x, y, 40, 15, 2);
        yield return (x, y) => new Enemy("Cultist", 'C', x, y, 35, 12, 1);
    }

    public IEnumerable<IDungeonStep> CreateGenerationStrategy()
    {
        return new List<IDungeonStep> {
            new FilledDungeon(),
            new StartingExit(),
            new RandomCorridor(100), 
            new ItemsGenerator(CreateItemPool(), 10),
            new ArtifactGenerator(CreateArtifact()),
            new EnemyGenerator(CreateEnemyPool(), 6)
        };
    }
}