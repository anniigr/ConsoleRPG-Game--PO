<p align="center">
  <!-- Replace the URL below with the path to your gameplay GIF later -->
  <img src="https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjEx.../giphy.gif" width="550px" alt="Gameplay Preview"/>
</p>

# ⚔️ Console RPG

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/Console-000000?style=for-the-badge&logo=windows-terminal&logoColor=white" alt="Terminal"/>
</p>

## 📘 Table of Contents

1. [📖 Overview](#-overview)
2. [🎯 Features](#-features)
3. [🎮 Controls](#-controls)
4. [🏗️ Architecture](#-architecture)

## 📖 Overview

**Console RPG** is a classic retro-style role-playing game that runs entirely inside your terminal window[cite: 2]. Built from scratch in C#, this project showcases robust Object-Oriented Programming (OOP) concepts, encapsulation, and clean architectural design[cite: 2]. 

Players can explore a dynamic map grid, gather treasure, manage an active inventory, and dual-wield weapons to calculate total combat damage[cite: 2]. No heavy graphics engines required—just pure, retro console mechanics[cite: 2]!

## 🎯 Features

> 👇 **Click on any feature below to expand and see details**

<details>
<summary>
🗺️ <strong> Grid Map Exploration </strong> - Navigate a structured map layout
</summary>
<br>
<ul>
  <li>&nbsp Explorable grid boundaries measuring <strong>40x20</strong> cells[cite: 2].</li>
  <li>&nbsp Distinct map elements: solid walls (<code>█</code>) and open floor layouts (<code> </code>)[cite: 2].</li>
  <li>&nbsp Fully active collision system preventing the player (<code>¶</code>) from walking through impassable terrain[cite: 2].</li>
</ul>
</details>

<details>
<summary>
🎒 <strong> Interactive Equipment & Inventory </strong> - Manage your combat gear
</summary>
<br>
<ul>
  <li>&nbsp <strong>One-Handed Modularity:</strong> Weapons like Swords (<code>†</code>) and Axes (<code>P</code>) can be assigned freely to either the left or right hand[cite: 2].</li>
  <li>&nbsp <strong>Two-Handed Mastery:</strong> Massive Greatswords (<code>W</code>) dynamically occupy both hands simultaneously[cite: 2].</li>
  <li>&nbsp <strong>Dynamic Damage Calculation:</strong> Combines active weapon ratings automatically to output your total offensive power[cite: 2].</li>
</ul>
</details>

<details>
<summary>
💰 <strong> Automatic Currency Wallet </strong> - Gather rare loot
</summary>
<br>
<ul>
  <li>&nbsp Walking over currency items auto-processes them into your stash[cite: 2].</li>
  <li>&nbsp Finding a Coin (<code>o</code>) instantly awards +5 coins[cite: 2].</li>
  <li>&nbsp Uncovering Gold (<code>G</code>) increases your permanent gold bar inventory[cite: 2].</li>
</ul>
</details>

<details>
<summary>
📊 <strong> Live Stat Monitoring & HUD </strong> - Tracking status in real-time
</summary>
<br>
<p>
The UI features a side-panel HUD constructed via high-performance <code>StringBuilder</code> streams[cite: 2]. It displays live stats (HP, Strength, Dexterity, Luck, Aggression, Wisdom), current active equipment hand slots, environmental floor items, and a rolling text log tracking your actions[cite: 2].
</p>
</details>

## 🎮 Controls

The game operates seamlessly across two primary interface modes[cite: 2]:

### 🗺️ Map Mode Controls
* **`W` / `A` / `S` / `D`** — Move Player Up, Left, Down, or Right[cite: 2].
* **`E`** — Pick up the topmost item on the current cell floor[cite: 2].
* **`I`** — Access the Equipment Inventory screen[cite: 2].
* **`Q` / `Esc`** — Safe terminate and close game instance[cite: 2].

### 🎒 Inventory Mode Controls
* **`W` / `S`** — Move selection cursor up and down through item list[cite: 2].
* **`L`** — Equip the highlighted item to your Left Hand[cite: 2].
* **`R`** — Equip the highlighted item to your Right Hand[cite: 2].
* **`1`** — Unequip item currently held in your Left Hand[cite: 2].
* **`2`** — Unequip item currently held in your Right Hand[cite: 2].
* **`Q`** — Drop selected inventory item back onto the map floor cell[cite: 2].
* **`I` / `Esc`** — Close inventory view and return to exploration[cite: 2].

## 🏗️ Architecture

The backend of **Console RPG** uses structured namespacing to segregate data models from drawing mechanics[cite: 2]:

* **`ConsoleRPG.World`** — Governs tile maps, coordinate cells, and spatial terrain classes[cite: 2].
* **`ConsoleRPG.Entities`** — Houses player metrics, movement systems, and baseline statistics[cite: 2].
* **`ConsoleRPG.Items`** — Handles items, separating trash junk from scalable weapon frameworks and wallets[cite: 2].
* **`ConsoleRPG.Engine`** — Drives execution; contains the core `GameEngine` loop and frame-by-frame text rendering pipelines[cite: 2].

### 📊 Item Hierarchy Blueprint

The following entity tree maps how classes inherit behaviors from the master object types:

```mermaid
classDiagram
    class Item {
        <<abstract>>
        +char Symbol
        +string Name
        +GetDamage() int
        +PickUp()
        +Drop()
    }
    
    class Weapon {
        <<abstract>>
        +int damage
    }
    
    class Currency {
        <<abstract>>
        +ApplyToWallet(Player p)
    }

    class OneHandedWeapon {
        <<abstract>>
        +EquipLeft()
        +EquipRight()
    }
    
    class TwoHandedWeapon {
        <<abstract>>
        +EquipLeft()
        +EquipRight()
    }

    Item <|-- Weapon
    Item <|-- Currency
    Item <|-- Bone
    Item <|-- Mug
    Item <|-- Rope
    
    Weapon <|-- OneHandedWeapon
    Weapon <|-- TwoHandedWeapon
    
    Currency <|-- Coin
    Currency <|-- Gold
    
    OneHandedWeapon <|-- Sword
    OneHandedWeapon <|-- Axe
    TwoHandedWeapon <|-- Greatsword
