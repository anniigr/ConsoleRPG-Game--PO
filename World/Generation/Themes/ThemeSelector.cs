using ConsoleRPG.Config;
namespace ConsoleRPG.World;
public static class ThemeSelector {
    public static IDungeonThemeFactory SelectTheme(string themeName)
    {
        return themeName.ToLower() switch
        {
            "vault" => new VaultThemeFactory(),
            "library" => new LibraryThemeFactory(),
            "scifi"   => new SciFiThemeFactory(),
            _         => new SciFiThemeFactory()
        };
    }
    
}