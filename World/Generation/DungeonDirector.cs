namespace ConsoleRPG.World;


public class DungeonDirector
{
    private IDungeonBuilder _builder;
    public DungeonDirector(IDungeonBuilder builder)
    {
        _builder = builder;
    }
    public Map CreateThemeMap(IDungeonThemeFactory theme)
    {
       return _builder.Build(theme);
    }
  

}