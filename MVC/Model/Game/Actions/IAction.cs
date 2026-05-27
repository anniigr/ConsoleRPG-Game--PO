using ConsoleRPG.Engine;
// Command
public interface IAction
{
    string Name {get;}
    ConsoleKey Key {get;}
    bool IsExecutable(GameEngine engine);
    void Execute(GameEngine engine);
}
public class ChangeStateAction : IAction
{
    private readonly Func<IGameState> _stateFactory;
    public string Name { get; }
    public ConsoleKey Key { get; }

    public ChangeStateAction(string name, ConsoleKey key, Func<IGameState> stateFactory)
    {
        Name = name;
        Key = key;
        _stateFactory = stateFactory;
    }

    public bool IsExecutable(GameEngine engine) => true;
    public void Execute(GameEngine engine) => engine.ChangeState(_stateFactory());
}

