using ConsoleRPG.Engine;
namespace ConsoleRPG.MVC.View;
public interface IGameView
{
    public void DrawFrame(GameEngine engine, IGameState currentState);
    public void DrawLog();
    public void DrawHistory(List<string> logs);
}