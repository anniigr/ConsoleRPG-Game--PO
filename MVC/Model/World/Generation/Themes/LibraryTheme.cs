// ===== ./World/Generation/Themes/LibraryThemeFactory.cs =====
using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using System.Collections.Generic;
using ConsoleRPG.Entities.Observers;
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
        yield return () => new MagicStaff();
        yield return () => new Axe();
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
            new RandomCorridor(160), 
            new ItemsGenerator(CreateItemPool(), 30),
            new ArtifactGenerator(CreateArtifact()),
            new EnemyGenerator(CreateEnemyPool(), 6)
        };
    }
}