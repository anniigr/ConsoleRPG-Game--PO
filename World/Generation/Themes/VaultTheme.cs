// ===== ./World/Generation/Themes/VaultThemeFactory.cs =====
using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using System.Collections.Generic;
using System;

namespace ConsoleRPG.World;

public class VaultThemeFactory : IDungeonThemeFactory
{
    public string GetGreetingMessage() => "Czujesz swędzenie w portfelu. Złoto jest blisko...";
    public (int width, int height) GetSize() => (45, 25);

    public Item CreateArtifact() => new LuckyCoinPouch();

    public IEnumerable<Func<Item>> CreateItemPool()
    {
        yield return () => new Coin();
        yield return () => new Gold();
        yield return () => new Sword();
    }

    public IEnumerable<Func<int, int, Enemy>> CreateEnemyPool()
    {
        yield return (x, y) => new Enemy("Mimic Safe", 'S', x, y, 80, 15, 10);
        yield return (x, y) => new Enemy("Aggressive Briefcase", 'B', x, y, 30, 10, 0);
    }

    public IEnumerable<IDungeonStep> CreateGenerationStrategy()
    {
        return new List<IDungeonStep> {
            new FilledDungeon(),
            new StartingExit(),
            new CentralRoom(15, 10), 
            new RandomCorridor(150),
            new ItemsGenerator(CreateItemPool(), 15), 
            new ArtifactGenerator(CreateArtifact()),
            new EnemyGenerator(CreateEnemyPool(), 5)
        };
    }
}