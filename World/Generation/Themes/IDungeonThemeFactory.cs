using ConsoleRPG.Entities;
using ConsoleRPG.Items;
using System.Collections.Generic;
using System;

namespace ConsoleRPG.World;

public interface IDungeonThemeFactory
{
    string GetGreetingMessage();
    (int width, int height) GetSize();

    IEnumerable<IDungeonStep> CreateGenerationStrategy();
    IEnumerable<Func<Item>> CreateItemPool();
    Item CreateArtifact();
    IEnumerable<Func<int, int, Enemy>> CreateEnemyPool();
}