using System;
using ConsoleRPG.Entities;
using ConsoleRPG.World;

namespace ConsoleRPG.Engine
{
    public class GameEngine
    {
        private Map map;
        private Player player;
        private Renderer renderer;
        private bool isRunning;
        private bool isInventoryMode;
        private int invCursor;

        public GameEngine()
        {
            map = new Map();
            player = new Player(0,0);
            renderer = new Renderer(map, player);
            isRunning = true;
            isInventoryMode = false;
        }

        public void Run()
        {
            while (isRunning)
            {
                renderer.DrawFrame(isInventoryMode, invCursor);
                HandleInput();
            }
        }

        private void HandleInput()
        {
            var keyInfo = Console.ReadKey(true);
            var key = keyInfo.Key;

            if (isInventoryMode)
                HandleInventoryInput(key);
            else
                HandleMapInput(key);
        }

        private void HandleMapInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W: player.Move(0, -1, map); break;
                case ConsoleKey.S: player.Move(0, 1, map); break;
                case ConsoleKey.A: player.Move(-1, 0, map); break;
                case ConsoleKey.D: player.Move(1, 0, map); break;
                case ConsoleKey.E: player.PickUp(map); break;
                case ConsoleKey.I: 
                    isInventoryMode = true; 
                    invCursor = 0;
                    player.LogMessage = "Equipment opened";
                    break;
                case ConsoleKey.Q: 
                case ConsoleKey.Escape: 
                    isRunning = false; 
                    break;
            }
        }

        private void HandleInventoryInput(ConsoleKey key)
        {
            if (player.Inventory.Count == 0) invCursor = 0;
            else if (invCursor >= player.Inventory.Count) invCursor = player.Inventory.Count - 1;

            switch (key)
            {
                case ConsoleKey.W: 
                    if (invCursor > 0) invCursor--; 
                    break;
                case ConsoleKey.S: 
                    if (invCursor < player.Inventory.Count - 1) invCursor++; 
                    break;
                case ConsoleKey.L:
                    if (player.Inventory.Count > 0) player.Inventory[invCursor].EquipLeft(player);
                    break;
                case ConsoleKey.R:
                    if (player.Inventory.Count > 0) player.Inventory[invCursor].EquipRight(player);
                    break;
                case ConsoleKey.Q:
                    if (player.Inventory.Count > 0)
                    {
                        player.DropItem(player.Inventory[invCursor], map);
                        if (invCursor > 0) invCursor--; 
                    }
                    break;
                case ConsoleKey.D1: player.UnequipLeft(); break;
                case ConsoleKey.D2: player.UnequipRight(); break;
                case ConsoleKey.I:
                case ConsoleKey.Escape:
                    isInventoryMode = false;
                    player.LogMessage = "Equipment closed";
                    break;
            }
        }
    }
}