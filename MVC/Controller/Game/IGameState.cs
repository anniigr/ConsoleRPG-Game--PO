using ConsoleRPG.Combat;
using ConsoleRPG.Log;
using ConsoleRPG.MVC.View;
namespace ConsoleRPG.Engine;
public interface IGameState
{
    public void Update(GameEngine engine, ConsoleKey key);
    public void Draw(IGameView view, GameEngine engine);
    ActionManager GetActionManager();
    string GetPanelLine(int lineIndex, GameEngine engine);
}

public class MapState : IGameState
{
    private readonly ActionManager _actions;
    public ActionManager GetActionManager() => _actions;
    public List<IAction> GetActions(GameEngine engine) => _actions.GetAvailableActions(engine);
    public MapState()
    {
        _actions = new ActionManager();
        _actions.AddAction(new MoveUp());
        _actions.AddAction(new MoveDown());
        _actions.AddAction(new MoveLeft());
        _actions.AddAction(new MoveRight());
        _actions.AddAction(new PickUpAction());
        _actions.AddAction(new QuitGame());
        _actions.AddAction(new ChangeStateAction("Inventory", ConsoleKey.I, () => new InventoryState()));
        _actions.AddAction(new ChangeStateAction("Help", ConsoleKey.H, () => new HelpState(this)));

        _actions.AddAction(new SpecialAttackAction("Normal Attack", ConsoleKey.D1, p => new NormalAttack(p)));
        _actions.AddAction(new SpecialAttackAction("Stealth Attack", ConsoleKey.D2, p => new StealthAttack(p)));
        _actions.AddAction(new SpecialAttackAction("Magic Attack", ConsoleKey.D3, p => new MagicAttack(p)));
        _actions.AddAction(new ChangeStateAction("Dziennik Zdarzeń", ConsoleKey.J, () => new LogHistoryState(this)));
    }
    public void Update(GameEngine engine, ConsoleKey key)
    {
        var action = _actions.FindAction(key);
        if (action != null)
        {
            action.Execute(engine);
            engine.ProcessEnemyTurn();
        }
        else
        {
            engine.ExcecuteAction(key); 
        }
    }

    public void Draw(IGameView view, GameEngine engine)
    {
        view.DrawFrame(engine,this);
        view.DrawLog();
    }
    public string GetPanelLine(int lineIndex, GameEngine engine)
    {
        return lineIndex switch
        {
            0 => "--- STATISTICS ---",
                        1 => $"HP: {engine.player.Health} | Strength: {engine.player.Strength} | Dexterity: {engine.player.Dexterity}",
                        2 => $"Luck: {engine.player.Luck} | Aggression: {engine.player.Aggression} | Wisdom: {engine.player.Wisdom}",
                        3 => $"Coins: {engine.player.Coins} | Gold: {engine.player.Gold}",
                        4 => "--- EQUIPMENT (Press I) ---",
                        5 => $"Left arm:  {(engine.player.LeftHand != null ? engine.player.LeftHand.Name : "[Empty]")}",
                        6 => $"Right arm: {(engine.player.RightHand != null ? engine.player.RightHand.Name : "[Empty]")}",
                        7 => $"Damage:  {engine.player.GetTotalDamage()}",
                        8 => "--- ENVIRONMENT ---",
                        9 => engine.GetFloorInfo(),
                        10 => "[H] Help",

                        _ => ""
        };
    }
}


public class InventoryState : IGameState
{
    private readonly ActionManager _actions;
    public ActionManager GetActionManager() => _actions;
    public int InventoryCursor {get; private set;}
    public List<IAction> GetActions(GameEngine engine) => _actions.GetAvailableActions(engine);
    public InventoryState()
    {
        _actions = new ActionManager();
        _actions.AddAction(new InventoryMoveUp(this));
        _actions.AddAction(new InventoryMoveDown(this));
        _actions.AddAction(new EquipLeftAction(this));
        _actions.AddAction(new EquipRightAction(this));
        _actions.AddAction(new DropItemAction(this));
        _actions.AddAction(new UnequipLeftAction());
        _actions.AddAction(new UnequipRightAction());
        _actions.AddAction(new ChangeStateAction("Inventory", ConsoleKey.I, () => new MapState()));
        _actions.AddAction(new ChangeStateAction("Help", ConsoleKey.H, () => new HelpState(this)));


    }
    public void Update(GameEngine engine, ConsoleKey key)
    {
         _actions.FindAction(key)?.Execute(engine);
    }
    public void MoveCursor(int delta, int maxItems)
    {
        InventoryCursor += delta;
        if (InventoryCursor < 0) InventoryCursor = 0;
        if (maxItems > 0 && InventoryCursor >= maxItems) InventoryCursor = maxItems - 1;
    }
    public void Draw(IGameView view, GameEngine engine)
    {
        view.DrawFrame(engine,this);
        view.DrawLog();
    }
    public string GetPanelLine(int lineIndex, GameEngine engine)
    {
        if (lineIndex == 0) return "--- EQUIPMENT MODE ---";
        int itemIdx = lineIndex - 2;
        if (itemIdx >= 0 && itemIdx < engine.player.Inventory.Count)
        {
            string pointer = (itemIdx == InventoryCursor) ? "> " : "  ";
            return $"{pointer}{engine.player.Inventory[itemIdx].Name}";
        }
        if (lineIndex == engine.player.Inventory.Count + 3 ) return "[H] Help";

        return "";
    }
}

public class HelpState : IGameState
{
    private readonly IGameState _previousState;
    public HelpState(IGameState previousState)
    {
        _previousState = previousState;
    }

    public void Update(GameEngine engine, ConsoleKey key)
    {
        if (key == ConsoleKey.H || key == ConsoleKey.Escape)
        {
            engine.ChangeState(_previousState);
        }
    }

    public void Draw(IGameView view, GameEngine engine)
    {
        view.DrawFrame(engine, this);
        view.DrawLog();
    }
    public ActionManager GetActionManager() => _previousState.GetActionManager();
    public string GetPanelLine(int lineIndex, GameEngine engine)
    {
        var actions = _previousState.GetActionManager().GetAvailableActions(engine);
        if (lineIndex == 0) return "--- HELP ---";
        int actionIdx = lineIndex - 2;
        if (actionIdx >= 0 && actionIdx < actions.Count) 
            return $"[{actions[actionIdx].Key}] {actions[actionIdx].Name}";
        return "";
    }
}
public class LogHistoryState : IGameState
{
    private readonly IGameState _previousState;

    public LogHistoryState(IGameState previousState) => _previousState = previousState;

    public void Update(GameEngine engine, ConsoleKey key)
    {
        if (key == ConsoleKey.J || key == ConsoleKey.Escape)
            engine.ChangeState(_previousState);
    }

    public void Draw(IGameView view, GameEngine engine)
    {
        var logs = GameLogger.GetInstance().GetAllLogs();
        view.DrawHistory(logs);
    }

    public ActionManager GetActionManager() => _previousState.GetActionManager();
    public string GetPanelLine(int lineIndex, GameEngine engine) => "";
}