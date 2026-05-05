// ===== ./World/Generation/Themes/SciFiThemeFactory.cs =====
using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using System.Collections.Generic;
using System;
using ConsoleRPG.Entities.Observers;

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
            new RandomRoom(5),
            new RandomCorridor(50), 
            new ItemsGenerator(CreateItemPool(), 8),
            new ArtifactGenerator(CreateArtifact()),
            new EnemyGenerator(CreateEnemyPool(), 8)
        };
    }
}