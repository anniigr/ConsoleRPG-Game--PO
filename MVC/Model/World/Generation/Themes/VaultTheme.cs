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

    public Item CreateArtifact()
    {
        var sword = new SlottedSword(damage: 2, slotCount: 2);

        var luckStone = new LuckStone();      
        sword.TryInsert(luckStone, 0); 

        var outerHandle = new Handle(2);     
        var strengthStone = new StrengthStone(); 
        outerHandle.TryInsert(strengthStone, 0);

        var innerHandle = new Handle(2);        
        var wisdomStone = new WisdomStone();   
        innerHandle.TryInsert(wisdomStone, 1);  
        outerHandle.TryInsert(innerHandle, 1);

        sword.TryInsert(outerHandle, 1);  

        return sword;
    }
    public IEnumerable<Func<Item>> CreateItemPool()
    {
        yield return () => new Coin();
        yield return () => new Gold();
        yield return () => new Sword();
        yield return () => new StrengthStone();  
        yield return () => new LuckStone();       
        yield return () => new WisdomStone();     
        yield return () => new Handle(2);         
    }

    public IEnumerable<SpeciesSpawnDefinition> CreateEnemyPool()
    {
        EventManagerDeath groupZombies = new EventManagerDeath("Zombi");
        EventManagerDeath groupSkeleton = new EventManagerDeath("Skeleton");
        EventManagerDeath groupGoblin = new EventManagerDeath("Goblin");

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
        yield return new SpeciesSpawnDefinition((x, y) => { 
            var goblin = new Goblin(x, y);
            goblin.group = groupZombies;
            groupGoblin.Subscribe(goblin);
            return goblin;
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
        new ArtifactGenerator(new Handle(2)),              
        new ArtifactGenerator(new Handle(3)),              
        new EnemyGenerator(CreateEnemyPool(), 5)
    };
    }
}