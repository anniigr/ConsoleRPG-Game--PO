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

    public Map Build() {
        var map = new Map(_w, _h);
        foreach (var step in _steps) step.Apply(map);
        Console.WriteLine("[LOG] Map builded.");
        return map;
    }
}