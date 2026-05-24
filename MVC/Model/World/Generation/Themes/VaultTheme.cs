// ===== ./World/Generation/Themes/VaultThemeFactory.cs =====
using ConsoleRPG.Entities;
using ConsoleRPG.Entities.Observers;
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

    public IEnumerable<SpeciesSpawnDefinition> CreateEnemyPool()
    {
        EventManagerDeath groupZombies = new EventManagerDeath("Zombi");
        EventManagerDeath groupSkeleton = new EventManagerDeath("Skeleton");

        yield return new SpeciesSpawnDefinition((x, y) => { 
            var zombie = new Zombie(x, y);
            zombie.group = groupZombies;
            groupZombies.Subscribe(zombie);
            return zombie;
        }, 3); 

        yield return new SpeciesSpawnDefinition((x, y) => { 
            var skeleton = new Skeleton(x, y);
            skeleton.group = groupSkeleton;
            groupSkeleton.Subscribe(skeleton);
            return skeleton;
        }, 3);
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