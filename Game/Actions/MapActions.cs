using System;
using ConsoleRPG.Engine;
using ConsoleRPG.Combat;
using ConsoleRPG.Entities;
namespace ConsoleRPG.Engine
{
    public abstract class MoveAction : IAction
    {
        public abstract string Name { get; }
        public abstract ConsoleKey Key { get; }
        protected abstract int DX { get; }
        protected abstract int DY { get; }

        public bool IsExecutable(GameEngine engine)
        {
            var targetCell = engine.map.GetCell(engine.player.X + DX, engine.player.Y + DY);
            return targetCell != null && targetCell.Terrain.IsPassable();
        }

        public void Execute(GameEngine engine)
        {
            engine.player.Move(DX, DY, engine.map);
            if (engine.player.Health <= 0)
            {
                engine.player.LogMessage = "YOU ARE DEAD";
                engine.Quit(); 
            }
        }
    }

    public class MoveUp : MoveAction {
        public override string Name => "Move Up";
        public override ConsoleKey Key => ConsoleKey.W;
        protected override int DX => 0;
        protected override int DY => -1;
    }

    public class MoveDown : MoveAction {
        public override string Name => "Move Down";
        public override ConsoleKey Key => ConsoleKey.S;
        protected override int DX => 0;
        protected override int DY => 1;
    }

    public class MoveLeft : MoveAction {
        public override string Name => "Move Left";
        public override ConsoleKey Key => ConsoleKey.A;
        protected override int DX => -1;
        protected override int DY => 0;
    }

    public class MoveRight : MoveAction {
        public override string Name => "Move Right";
        public override ConsoleKey Key => ConsoleKey.D;
        protected override int DX => 1;
        protected override int DY => 0;
    }

    public class PickUpAction : IAction
    {
        public string Name => "Pick Up Item";
        public ConsoleKey Key => ConsoleKey.E;
        
        public bool IsExecutable(GameEngine engine) => 
            engine.map.GetCell(engine.player.X, engine.player.Y).Items.Count > 0;

        public void Execute(GameEngine engine) => engine.player.PickUp(engine.map);
    }

    public class SpecialAttackAction : IAction {
    private readonly Func<Player, IAttackVisitor> _visitorFactory;
    public string Name { get; }
    public ConsoleKey Key { get; }

    public SpecialAttackAction(string name, ConsoleKey key, Func<Player, IAttackVisitor> factory) {
        Name = name; Key = key; _visitorFactory = factory;
    }

    public bool IsExecutable(GameEngine engine) {
    int[] dx = { 0, 0, 0, -1, 1 };
    int[] dy = { 0, -1, 1, 0, 0 };

    for (int i = 0; i < 5; i++) {
        var cell = engine.map.GetCell(engine.player.X + dx[i], engine.player.Y + dy[i]);
        if (cell != null && cell.Enemy != null) return true;
    }
    return false;
}

public void Execute(GameEngine engine) {
    int[] dx = { 0, 0, 0, -1, 1 };
    int[] dy = { 0, -1, 1, 0, 0 };

    for (int i = 0; i < 5; i++) {
        var cell = engine.map.GetCell(engine.player.X + dx[i], engine.player.Y + dy[i]);
        if (cell != null && cell.Enemy != null) {
            var enemy = cell.Enemy;
            engine.player.AttackEnemy(enemy, _visitorFactory(engine.player));
            
            if (enemy.Health <= 0) cell.Enemy = null;
            return; 
        }
    }
}
}
    public class QuitGame : IAction
    {
        public string Name => "Quit Game";
        public ConsoleKey Key => ConsoleKey.Q;
        public bool IsExecutable(GameEngine engine) => true;
        public void Execute (GameEngine engine) => engine.Quit();  
    }
}