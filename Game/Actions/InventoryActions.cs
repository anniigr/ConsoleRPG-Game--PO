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
        public void Execute (GameEngine engine) => engine.player.UnequipRight();
        
    }
    public class UnequipLeftAction : IAction
    {
        public string Name => "Unequip Left";
        public ConsoleKey Key => ConsoleKey.D1;
        public bool IsExecutable(GameEngine engine) => engine.player.LeftHand != null;
        public void Execute (GameEngine engine) => engine.player.UnequipLeft();
        
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
                {
                    _state.MoveCursor(-1, inv.Count);
                }
                else if (inv.Count == 0)
                {
                    _state.MoveCursor(0, 0); 
                }
            }
        }
    }
