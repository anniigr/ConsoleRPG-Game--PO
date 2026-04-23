// ===== ./World/Generation/Themes/SciFiThemeFactory.cs =====
using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using System.Collections.Generic;
using System;

namespace ConsoleRPG.World;

public class SciFiThemeFactory : IDungeonThemeFactory
{
    public string GetGreetingMessage() => "Zgrzyt metalu odbija się echem od ścian.";
    public (int width, int height) GetSize() => (50, 25);

    public Item CreateArtifact() => new Blaster();

    public IEnumerable<Func<Item>> CreateItemPool()
    {
        yield return () => new MetalScrap();
        yield return () => new Rope();
    }

    public IEnumerable<Func<int, int, Enemy>> CreateEnemyPool()
    {
        yield return (x, y) => new Enemy("Cleaner Robot", 'R', x, y, 60, 20, 5);
        yield return (x, y) => new Enemy("Security Drone", 'D', x, y, 45, 25, 3);
    }

    public IEnumerable<IDungeonStep> CreateGenerationStrategy()
    {
        return new List<IDungeonStep> {
            new FilledDungeon(),
            new StartingExit(),
            new RandomRoom(5),
            new RandomCorridor(50), 
            new ItemsGenerator(CreateItemPool(), 8),
            new ArtifactGenerator(CreateArtifact()),
            new EnemyGenerator(CreateEnemyPool(), 8)
        };
    }
}