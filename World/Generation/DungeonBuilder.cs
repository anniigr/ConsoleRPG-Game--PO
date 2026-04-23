namespace ConsoleRPG.World;

public class DungeonBuilder : IDungeonBuilder
{
    List<IDungeonStep> steps;
    public DungeonBuilder()
    {
        steps = new List<IDungeonStep>();
    }
    public Map Build(IDungeonThemeFactory themeFactory)
    {
        var steps = themeFactory.CreateGenerationStrategy();
        if (steps.Count() == 0)
        throw new InvalidOperationException("Dungeon must have at least one step.");

        if (!(steps.First() is EmptyDungeon || steps.First() is FilledDungeon))
        throw new InvalidOperationException("First step must be EmptyDungeon or FilledDungeon.");
        
        (int width, int height) = themeFactory.GetSize();
        var map = new Map(width, height);
        
        foreach (var step in steps)
        {
            step.Apply(map);
        }
        return map;
    }
} 
