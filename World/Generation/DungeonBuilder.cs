namespace ConsoleRPG.World;

public class DungeonBuilder : IDungeonBuilder
{
    List<IDungeonStep> steps;
    private int width = 40;
    private int height = 20;

    public IDungeonBuilder SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        return this;
    }
    public DungeonBuilder()
    {
        steps = new List<IDungeonStep>();
    }
    public IDungeonBuilder AddStep(IDungeonStep step)
    {
        steps.Add(step);
        return this;
    }
    public Map Build()
    {
        if (steps.Count == 0)
        throw new InvalidOperationException("Dungeon must have at least one step.");

        if (!(steps[0] is EmptyDungeon || steps[0] is FilledDungeon))
        throw new InvalidOperationException("First step must be EmptyDungeon or FilledDungeon.");

        var map = new Map(width, height);
        foreach (var step in steps)
        {
            step.Apply(map);
        }
        return map;
    }
} 
