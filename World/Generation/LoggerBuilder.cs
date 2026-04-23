namespace ConsoleRPG.World;
public class LoggingDungeonBuilder : IDungeonBuilder 
{
    private int _w, _h;
    private List<IDungeonStep> _steps = new List<IDungeonStep>();

    public IDungeonBuilder AddStep(IDungeonStep step) 
    {
        _steps.Add(step);
        Console.WriteLine($"[LOG] {step.GetDescription()}"); 
        Thread.Sleep(800);
        return this;
    }

    public IDungeonBuilder SetSize(int width, int height) {
        _w = width; _h = height;
        Console.WriteLine($"[LOG] Size set: {width}x{height}");
        return this;
    }

    public Map Build(IDungeonThemeFactory themeFactory) {
        var steps = themeFactory.CreateGenerationStrategy();
        if (steps.Count() == 0)
        throw new InvalidOperationException("Dungeon must have at least one step.");

        if (!(steps.First() is EmptyDungeon || steps.First() is FilledDungeon))
        throw new InvalidOperationException("First step must be EmptyDungeon or FilledDungeon.");
        
        (int width, int height) = themeFactory.GetSize();
        var map = new Map(width, height);
        
        foreach (var step in steps)
        {
            this.AddStep(step);
            step.Apply(map);
        }

        Console.WriteLine("[LOG] Map builded.");
        return map;

    }
}