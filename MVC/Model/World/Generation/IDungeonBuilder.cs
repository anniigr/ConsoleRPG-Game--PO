namespace ConsoleRPG.World;

public interface IDungeonBuilder
{
    public Map Build(IDungeonThemeFactory themeFactory);
}