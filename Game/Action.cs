using ConsoleRPG.Engine;
using ConsoleRPG.World;

public interface IAction
{
    string Name {get;}
    ConsoleKey key {get;}
    bool isExecutable(GameEngine engine);
    void Execute(GameEngine engine);
}

public class MoveUp : IAction
{
    public string Name => "Move player/cursor up";
    public ConsoleKey key => ConsoleKey.W;
    public bool isExecutable(GameEngine engine)
    {
        if (engine.isInventoryOpened && engine.InventoryCursor <= 0) return false;
        return true;
    } 
    public void Execute (GameEngine engine)
    {
        if (engine.isInventoryOpened) 
            engine.MoveInventoryCursorUp();
        else 
            engine.player.Move(0,-1,engine.map);
    }
}

public class MoveDown : IAction
{
    public string Name => "Move player/cursor down";
    public ConsoleKey key => ConsoleKey.S;
    public bool isExecutable(GameEngine engine)
    {
        if (engine.isInventoryOpened && engine.InventoryCursor >= engine.player.Inventory.Count - 1 ) return false;
        return true;
    }
    public void Execute (GameEngine engine)
    {
        if (engine.isInventoryOpened) 
            engine.MoveInventoryCursorDown();
        else 
            engine.player.Move(0,1,engine.map);
    }
}

public class MoveRight : IAction
{
    public string Name => "Move/Equipt right";
    public ConsoleKey key => ConsoleKey.D;
    public bool isExecutable(GameEngine engine) => !engine.isInventoryOpened;
    public void Execute (GameEngine engine)
    {
        engine.player.Move(1,0,engine.map);
    }
}
public class MoveLeft : IAction
{
    public string Name => "Move/Equipt left";
    public ConsoleKey key => ConsoleKey.A;
    public bool isExecutable(GameEngine engine) => !engine.isInventoryOpened;
    public void Execute (GameEngine engine)
    {
        engine.player.Move(-1,0,engine.map);
    }
}
public class EquipLeft : IAction
{
    public string Name => "Equipt left";
    public ConsoleKey key => ConsoleKey.L;
    public bool isExecutable(GameEngine engine) =>
    engine.isInventoryOpened &&
    engine.player.Inventory.Count > 0 &&
    engine.InventoryCursor >= 0 &&
    engine.InventoryCursor < engine.player.Inventory.Count;    public void Execute (GameEngine engine)
    {
        engine.player.Inventory[engine.InventoryCursor].EquipLeft(engine.player);
    }
}
public class EquipRight : IAction
{
    public string Name => "Equipt right";
    public ConsoleKey key => ConsoleKey.R;
    public bool isExecutable(GameEngine engine) =>
    engine.isInventoryOpened &&
    engine.player.Inventory.Count > 0 &&
    engine.InventoryCursor >= 0 &&
    engine.InventoryCursor < engine.player.Inventory.Count;
    public void Execute (GameEngine engine)
    {
        engine.player.Inventory[engine.InventoryCursor].EquipRight(engine.player);
    }
}

public class UnequipRight : IAction
{
     public string Name => "UnequipRight";
    public ConsoleKey key => ConsoleKey.D2;
    public bool isExecutable(GameEngine engine) => engine.isInventoryOpened && engine.player.RightHand != null;
    public void Execute (GameEngine engine) {
        if (engine.isInventoryOpened )
        {
            engine.player.UnequipRight();
        }      
    } 
}
public class UnequipLeft : IAction
{
    public string Name => "Unequip Left";
    public ConsoleKey key => ConsoleKey.D1;
    public bool isExecutable(GameEngine engine) => engine.isInventoryOpened && engine.player.LeftHand != null;    
    public void Execute (GameEngine engine) {
        if (engine.isInventoryOpened )
        {
            engine.player.UnequipLeft();
        }   
    } 
}
public class PickUp : IAction
{
    public string Name => "PickUp";
    public ConsoleKey key => ConsoleKey.E;
    public bool isExecutable(GameEngine engine) => engine.map.GetCell(engine.player.X, engine.player.Y).Items.Count > 0 ;
    public void Execute (GameEngine engine)
    {
        engine.player.PickUp(engine.map);
    }
}

public class InventorySwitch : IAction
{
    public string Name => "Inventory Switch";
    public ConsoleKey key => ConsoleKey.I;
    public bool isExecutable(GameEngine engine) => true;
    public void Execute (GameEngine engine)
    {
        engine.SetOtherInventoryMode();
    }
}

public class QuitGame : IAction
{
     public string Name => "Quit Game / throw down";
    public ConsoleKey key => ConsoleKey.Q;
    public bool isExecutable(GameEngine engine)
    {
        if (engine.isInventoryOpened )
        {
            return engine.player.Inventory.Count > 0;
        } 
        else return true;
    }
    public void Execute (GameEngine engine) {
        if (engine.isInventoryOpened )
        {
            engine.player.DropItem(engine.player.Inventory[engine.InventoryCursor], engine.map);
            if (engine.InventoryCursor > 0) engine.MoveInventoryCursorUp();
        }
        else engine.Quit();      
    } 
}

public class HelpSwitch : IAction
{
    public string Name => "Help";
    public ConsoleKey key => ConsoleKey.H;
    public bool isExecutable(GameEngine engine) => true;
    public void Execute(GameEngine engine)
    {
        engine.ToggleHelp();
    }
}



public class ActionManager
{
    private List<IAction> list = new List<IAction>();
    public void AddAction(IAction action)
    {
        list.Add(action);
    }
    public IAction? FindAction(ConsoleKey key)
    {
        return list.FirstOrDefault(a => a.key == key);
    }
    public List<IAction> GetAvailableActions(GameEngine engine)
    {
        return list.Where(a => a.isExecutable(engine)).ToList();
    }
}

