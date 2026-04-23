public class GameLogger
{
    private static ILogger _instance;

    private GameLogger() { }
    public static ILogger GetInstance(string name = "Player", string path = "./")
    {
        if (_instance == null)
        {
            _instance = new FileAndMemoryLogger(name,path);
        }
        return _instance;
    }
}