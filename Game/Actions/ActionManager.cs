using ConsoleRPG.Engine;
public class ActionManager
{
    private List<IAction> list = new List<IAction>();
    public void AddAction(IAction action)
    {
        list.Add(action);
    }
    public IAction? FindAction(ConsoleKey key)
    {
        return list.FirstOrDefault(a => a.Key == key);
    }
    public IAction? FindActionByType<T>() where T : IAction
    {
        return list.FirstOrDefault(a => a is T);
        
    }
    public List<IAction> GetAvailableActions(GameEngine engine)
    {
        return list.Where(a => a.IsExecutable(engine)).ToList();
    }
}

