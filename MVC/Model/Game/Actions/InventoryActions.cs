using System;
using ConsoleRPG.Engine;

namespace ConsoleRPG.Engine;

public class InventoryMoveUp : IAction
{
    private readonly InventoryState _state;
    public InventoryMoveUp(InventoryState state) => _state = state;
    public string Name => "Cursor Up";
    public ConsoleKey Key => ConsoleKey.W;
    public bool IsExecutable(GameEngine engine) => true;
    public void Execute(GameEngine engine) => _state.MoveCursor(-1, engine.player.Inventory.Count);
}

public class InventoryMoveDown : IAction
{
    private readonly InventoryState _state;
    public InventoryMoveDown(InventoryState state) => _state = state;
    public string Name => "Cursor Down";
    public ConsoleKey Key => ConsoleKey.S;
    public bool IsExecutable(GameEngine engine) => true;
    public void Execute(GameEngine engine) => _state.MoveCursor(1, engine.player.Inventory.Count);
}

public class EquipLeftAction : IAction
{
    private readonly InventoryState _state;
    public EquipLeftAction(InventoryState state) => _state = state;
    public string Name => "Equip Left";
    public ConsoleKey Key => ConsoleKey.L;
    public bool IsExecutable(GameEngine engine) => engine.player.Inventory.Count > 0;
    public void Execute(GameEngine engine) =>
        engine.player.Inventory[_state.InventoryCursor].EquipLeft(engine.player);
}

public class EquipRightAction : IAction
{
    private readonly InventoryState _state;
    public EquipRightAction(InventoryState state) => _state = state;
    public string Name => "Equip Right";
    public ConsoleKey Key => ConsoleKey.R;
    public bool IsExecutable(GameEngine engine) => engine.player.Inventory.Count > 0;
    public void Execute(GameEngine engine) =>
        engine.player.Inventory[_state.InventoryCursor].EquipRight(engine.player);
}

public class UnequipRightAction : IAction
{
    public string Name => "Unequip Right";
    public ConsoleKey Key => ConsoleKey.D2;
    public bool IsExecutable(GameEngine engine) => engine.player.RightHand != null;
    public void Execute(GameEngine engine) => engine.player.UnequipRight();
}

public class UnequipLeftAction : IAction
{
    public string Name => "Unequip Left";
    public ConsoleKey Key => ConsoleKey.D1;
    public bool IsExecutable(GameEngine engine) => engine.player.LeftHand != null;
    public void Execute(GameEngine engine) => engine.player.UnequipLeft();
}

public class DropItemAction : IAction
{
    private readonly InventoryState _state;
    public DropItemAction(InventoryState state) => _state = state;
    public string Name => "Drop Item";
    public ConsoleKey Key => ConsoleKey.Q;
    public bool IsExecutable(GameEngine engine) => engine.player.Inventory.Count > 0;

    public void Execute(GameEngine engine)
    {
        var inv = engine.player.Inventory;
        int idx = _state.InventoryCursor;

        if (inv.Count == 0) return;

        if (idx >= 0 && idx < inv.Count)
        {
            var item = inv[idx];
            var cell = engine.map.GetCell(engine.player.X, engine.player.Y);
            item.Drop(engine.player, cell);

            if (_state.InventoryCursor >= inv.Count && inv.Count > 0)
                _state.MoveCursor(-1, inv.Count);
            else if (inv.Count == 0)
                _state.MoveCursor(0, 0);
        }
    }
}

public class InsertIntoSlotAction : IAction
{
    private readonly InventoryState _state;
    public InsertIntoSlotAction(InventoryState state) => _state = state;
    public string Name => "Insert into slot [F]";
    public ConsoleKey Key => ConsoleKey.F;

    public bool IsExecutable(GameEngine engine)
    {
        if (engine.player.Inventory.Count == 0) return false;
        if (_state.InventoryCursor >= engine.player.Inventory.Count) return false;
        var item = engine.player.Inventory[_state.InventoryCursor];
        return item is ConsoleRPG.Items.ISlottable &&
               (engine.player.RightHand is ConsoleRPG.Items.IHasSlots ||
                engine.player.LeftHand is ConsoleRPG.Items.IHasSlots);
    }

    public void Execute(GameEngine engine)
    {
        var inv = engine.player.Inventory;
        if (inv.Count == 0) return;
        var item = inv[_state.InventoryCursor];

        if (item is not ConsoleRPG.Items.ISlottable slottable)
        {
            ConsoleRPG.Log.GameLogger.GetInstance().Log("Ten przedmiot nie może być wstawiany do slotu.");
            return;
        }

        ConsoleRPG.Items.IHasSlots? container = null;
        if (engine.player.RightHand is ConsoleRPG.Items.IHasSlots r) container = r;
        else if (engine.player.LeftHand is ConsoleRPG.Items.IHasSlots l) container = l;

        if (container == null)
        {
            ConsoleRPG.Log.GameLogger.GetInstance().Log("Brak broni/uchwytu ze slotami w ręce.");
            return;
        }

        for (int i = 0; i < container.SlotCount; i++)
        {
            if (container.Slots[i] == null && container.TryInsert(slottable, i))
            {
                inv.Remove(item);
                ConsoleRPG.Log.GameLogger.GetInstance().Log($"Wstawiono {item.Name} do slotu {i + 1}.");
                return;
            }
        }

        ConsoleRPG.Log.GameLogger.GetInstance().Log("Brak wolnych slotów w broni/uchwycie w ręce.");
    }
}