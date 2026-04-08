namespace ConsoleRPG.World;

public interface IDungeonBuilder
{
    public IDungeonBuilder SetSize(int width, int height);
    public IDungeonBuilder AddStep(IDungeonStep step);
    public Map Build();
}