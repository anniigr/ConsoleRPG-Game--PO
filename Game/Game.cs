using System;
using ConsoleRPG.Entities;
using ConsoleRPG.World;

namespace ConsoleRPG.Engine
{
    public class GameEngine
    {
        public Map map {get; private set;}
        public Player player {get; private set;}
        private Renderer renderer;
        private ActionManager actionManager;

        private bool isRunning;
        private bool isInventoryMode;
        private int invCursor;

        public int InventoryCursor => invCursor;
        public bool isInventoryOpened => isInventoryMode;
        private bool isHelpMode;
        public bool IsHelpOpened => isHelpMode;

        public void ToggleHelp()
        {
            isHelpMode = !isHelpMode;
            player.LogMessage = isHelpMode ? "Help opened" : "Help closed";
        }

        public GameEngine()
        {
            map = new Map();
            player = new Player(0,0);
            actionManager = new ActionManager();

            actionManager.AddAction(new MoveUp());
            actionManager.AddAction(new MoveDown());
            actionManager.AddAction(new MoveLeft());
            actionManager.AddAction(new MoveRight());
            actionManager.AddAction(new PickUp());
            actionManager.AddAction(new InventorySwitch());
            actionManager.AddAction(new QuitGame());
            actionManager.AddAction(new EquipLeft());
            actionManager.AddAction(new EquipRight());
            actionManager.AddAction(new UnequipLeft());
            actionManager.AddAction(new UnequipRight());
            actionManager.AddAction(new HelpSwitch());

            renderer = new Renderer(map, player, actionManager);
            isRunning = true;
            isInventoryMode = false;
        }

        public void Run()
        {
            while (isRunning)
            {
                renderer.DrawFrame(this);
                HandleInput();
            }
        }

        private void HandleInput()
        {
            var keyInfo = Console.ReadKey(true);
            var key = keyInfo.Key;

            if (IsHelpOpened)
            {
                if (key == ConsoleKey.H)
                {
                    ToggleHelp();
                }
                else
                {
                    player.LogMessage = "Press H to close help";
                }
                return;
            }

            var action = actionManager.FindAction(key);

            if (action == null)
                player.LogMessage = "Unknown key";
            else if (!action.isExecutable(this))
                player.LogMessage = "Action is not available";
            else
                action.Execute(this);
        }

        public void SetOtherInventoryMode()
        {
            if (isHelpMode)
            {
                player.LogMessage = "Close help first";
                return;
            }

            isInventoryMode = !isInventoryMode;
            if (isInventoryMode)
            {
                invCursor = 0;
                player.LogMessage = "Inventory opened";
            }
            else
            {
                player.LogMessage = "Inventory closed";
            }
        }
        public void MoveInventoryCursorUp() => invCursor--;
        public void MoveInventoryCursorDown() => invCursor++;
        
        public void Quit ()
        {
            isRunning = false;
        }
    }
}