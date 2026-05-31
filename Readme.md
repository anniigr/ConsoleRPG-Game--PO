# ⚔️ Console RPG: TCP Multiplayer & MVC Architecture

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/Architecture-MVC-blue?style=for-the-badge" alt="Architecture: MVC"/>
  <img src="https://img.shields.io/badge/Networking-TCP%20%2F%20JSON-orange?style=for-the-badge" alt="TCP Network"/>
  <img src="https://img.shields.io/badge/Clean%20Code-No%20RTTI-success?style=for-the-badge" alt="Clean Code"/>
</p>

## 📖 Overview

This project represents a scalable, real-time client-server system for multi-user interaction, implemented in C#. Throughout its 7-stage evolution, the architecture transitioned from a monolithic console-based game engine into a distributed system featuring an authoritative server and support for up to 9 concurrent network clients. The development focused on strict MVC-based decomposition, ensuring complete decoupling of business logic from presentation and networking layers.

The core engineering constraint of this project was absolute adherence to Object-Oriented Programming (OOP) principles. The codebase strictly avoids Run-Time Type Information (RTTI) such as `is`, `as`, `typeof`, and type-identifying `enums`. Instead, all logic is driven by clean polymorphism and classical Gang of Four (GoF) design patterns.

## 🏗️ Architectural Highlights

* **Model (Source of Truth):** Encapsulates the entire game state (map grid, player stats, NPC behavior, item locations). Entirely decoupled from the console and network layers.
* **View:** Implements separate renderers for the local console and remote network clients (via JSON payloads).
* **Controller:** Validates player input and routes actions to the Model. Each client session is handled by an isolated controller instance.
* **Concurrency & Networking:** Implemented using `TcpListener` and `TcpClient`. Thread safety is ensured through `lock` mechanisms and concurrent collections, allowing the server to handle multiple simultaneous TCP requests without race conditions.

---

## 🧩 Applied Design Patterns

| Design Pattern | Implementation in Project | Solved Engineering Problem |
| :--- | :--- | :--- |
| **Observer** | Sound Propagation & Herd AI | Allowed enemies to react to noise and intra-species deaths without tight coupling. Implemented *without* the C# `event` keyword to demonstrate deep OOP understanding. |
| **Composite** | Slotted Items & Sockets | Handled complex inventory math recursively (e.g., calculating stats for a sword holding a socket, which holds another socket and a magic stone). |
| **Decorator** | Dynamic Weapon Modifiers | Applied infinite stackable modifiers (e.g., "Unlucky", "Strong") to weapons during world generation without modifying base classes. |
| **Strategy / Builder** | Procedural Dungeon Generation | Enabled modular creation of dungeons based on specific configuration "Themes" (e.g., corridors, rooms, artifact placements). |
| **State** | Reactive Enemy AI | Allowed NPCs to dynamically switch behaviors (Follow, Flee, Ignore) based on a Sight > Sound priority hierarchy and HP thresholds. |
| **Singleton** | Configuration Manager / Logger | Ensures a single point of access to the external configuration files and global event logging stream across all modules. |
| **Director** | Dungeon Generation Control | Orchestrates the sequence of building steps (empty, corridors, central rooms, loot) defined in the Builder interface to produce consistent dungeon layouts. |
| **Visitor** | Combat & Statistic Calculation | Decouples the operation of calculating combat modifiers (or stat propagation) from the item hierarchy, allowing new operations without modifying Item classes. |


---

## 🚀 Key Features by Development Stage
<details>
<summary><strong>Stages 1–2: Engine & Procedural Generation</strong></summary>
Implemented a grid-based engine with modular dungeon generation using strategy-based building blocks (halls, rooms, artifacts).
</details>

```mermaid
classDiagram
    direction TB

    %% --- ENGINE & CORE ---
    class GameEngine {
        -Map map
        -Player player
        -Renderer renderer
        -bool isRunning
        -bool isInventoryMode
        -int invCursor
        +Run() void
        -HandleInput() void
        -HandleMapInput(ConsoleKey key) void
        -HandleInventoryInput(ConsoleKey key) void
    }

    class Renderer {
        -Map map
        -Player player
        +DrawFrame(bool isInventoryMode, int invCursor) void
        -GetLine(int lineIndex, string floorInfo, bool isInventoryMode, int invCursor) string
    }

    %% --- WORLD ---
    class Map {
        +int Width
        +int Height
        -Cell[,] grid
        +InitializeMap() void
        +GetCell(int i, int j) Cell
    }

    class Cell {
        +Terrain Terrain
        +List~Item~ Items
        +GetDrawSymbol() char
    }

    class Terrain {
        <<abstract>>
        +char Symbol*
        +IsPassable() bool*
    }

    class Floor {
        +char Symbol
        +IsPassable() bool
    }

    class Wall {
        +char Symbol
        +IsPassable() bool
    }

    %% --- ENTITIES ---
    class Player {
        +int X
        +int Y
        +int Strength
        +int Dexterity
        +int Health
        +int Luck
        +int Aggression
        +int Wisdom
        +int Coins
        +int Gold
        +Item? LeftHand
        +Item? RightHand
        +List~Item~ Inventory
        +string LogMessage
        +UnequipLeft() void
        +UnequipRight() void
        +GetTotalDamage() int
        +Move(int dx, int dy, Map map) void
        +PickUp(Map map) void
        +DropItem(Item item, Map map) void
    }

    %% --- ITEM BASE CLASSES ---
    class Item {
        <<abstract>>
        +char Symbol*
        +string Name*
        +GetDamage() int
        +EquipLeft(Player p) void
        +EquipRight(Player p) void
        +UnEquip(Player p) void
        +Drop(Player p, Cell cell) void
        +PickUp(Player p, Cell cell) void
    }

    class Weapon {
        <<abstract>>
        +int damage
        +GetDamage() int
    }

    class OneHandedWeapon {
        <<abstract>>
        +EquipLeft(Player p) void
        +EquipRight(Player p) void
        +UnEquip(Player p) void
    }

    class TwoHandedWeapon {
        <<abstract>>
        -EquipBoth(Player p) void
        +EquipLeft(Player p) void
        +EquipRight(Player p) void
        +UnEquip(Player p) void
    }

    class Currency {
        <<abstract>>
        +ApplyToWallet(Player p)*
        +PickUp(Player p, Cell cell) void
    }

    %% --- SPECIFIC ITEMS ---
    class Sword {
        +char Symbol
        +string Name
    }
    class Axe {
        +char Symbol
        +string Name
    }
    class Greatsword {
        +char Symbol
        +string Name
    }
    class Bone {
        +char Symbol
        +string Name
    }
    class Mug {
        +char Symbol
        +string Name
    }
    class Rope {
        +char Symbol
        +string Name
    }
    class Coin {
        +char Symbol
        +string Name
        +ApplyToWallet(Player p) void
    }
    class Gold {
        +char Symbol
        +string Name
        +ApplyToWallet(Player p) void
    }

    %% --- RELATIONSHIPS ---
    GameEngine *-- Map : "1 to 1"
    GameEngine *-- Player : "1 to 1"
    GameEngine *-- Renderer : "1 to 1"
    
    Renderer o-- Map : "Uses"
    Renderer o-- Player : "Uses"
    
    Map "1" *-- "many" Cell : "Composition"
    Cell "1" o-- "many" Item : "Aggregation"
    Cell --> Terrain : "Holds"
    
    Terrain <|-- Floor : "Inherits"
    Terrain <|-- Wall : "Inherits"
    
    Player "1" o-- "many" Item : "Inventory / Hands"

    Item <|-- Weapon
    Item <|-- Currency
    Item <|-- Bone
    Item <|-- Mug
    Item <|-- Rope

    Weapon <|-- OneHandedWeapon
    Weapon <|-- TwoHandedWeapon
    
    OneHandedWeapon <|-- Sword
    OneHandedWeapon <|-- Axe
    TwoHandedWeapon <|-- Greatsword

    Currency <|-- Coin
    Currency <|-- Gold
```
<details>
<summary><strong>Stage 3: Polymorphic Combat System</strong></summary>
Developed a combat model featuring weapon categories (Heavy, Light, Magic) and attack styles (Normal, Stealth, Magic). Damage scaling is strictly polymorphic.
</details>
```mermaid
classDiagram
    direction TB

    %% --- DECORATOR PATTERN: MODYFIKATORY PRZEDMIOTÓW ---
    class Item {
        <<abstract>>
        +string Name*
        +GetDamage() int
        +GetStatBonus() int
    }

    class ItemModifier {
        <<abstract>>
        #Item _wrappee
        +string Name
        +GetDamage() int
        +GetStatBonus() int
        +ItemModifier(Item wrappee)
    }

    class DamageModifier {
        -int _damageBonus
        +string Name
        +GetDamage() int
    }

    class StatModifier {
        -int _statBonus
        -string _statType
        +string Name
        +GetStatBonus() int
    }

    Item <|-- ItemModifier : Dziedziczenie (Decorator)
    ItemModifier o-- Item : Kompozycja (Owija bazowy Item)
    ItemModifier <|-- DamageModifier
    ItemModifier <|-- StatModifier

    %% --- KATEGORIE BRONI ---
    class Weapon {
        <<abstract>>
    }

    class HeavyWeapon {
        <<abstract>>
        +Accept(IAttackStrategy strategy, Player p) int
    }

    class LightWeapon {
        <<abstract>>
        +Accept(IAttackStrategy strategy, Player p) int
    }

    class MagicWeapon {
        <<abstract>>
        +Accept(IAttackStrategy strategy, Player p) int
    }

    Item <|-- Weapon
    Weapon <|-- HeavyWeapon
    Weapon <|-- LightWeapon
    Weapon <|-- MagicWeapon

    %% --- SYSTEM WALKI I WROGOWIE ---
    class Enemy {
        +int HealthPoints
        +int AttackValue
        +int ArmorPoints
        +TakeDamage(int damage) void
        +PerformCounterAttack(Player p) void
    }

    class Player {
        +PerformAttack(Enemy target, IAttackStrategy strategy) void
        +TakeDamage(int incomingDamage, IAttackStrategy lastUsedStrategy) void
    }

    %% --- VISITOR / STRATEGY PATTERN: MECHANIKA ATAKU I OBRONY ---
    class IAttackStrategy {
        <<interface>>
        +CalculateDamage(HeavyWeapon w, Player p) int
        +CalculateDamage(LightWeapon w, Player p) int
        +CalculateDamage(MagicWeapon w, Player p) int
        +CalculateDefense(Weapon w, Player p) int
    }

    class NormalAttack {
        +CalculateDamage(HeavyWeapon w, Player p) int
        +CalculateDamage(LightWeapon w, Player p) int
        +CalculateDamage(MagicWeapon w, Player p) int
        +CalculateDefense(Weapon w, Player p) int
    }

    class StealthAttack {
        +CalculateDamage(HeavyWeapon w, Player p) int
        +CalculateDamage(LightWeapon w, Player p) int
        +CalculateDamage(MagicWeapon w, Player p) int
        +CalculateDefense(Weapon w, Player p) int
    }

    class MagicAttack {
        +CalculateDamage(HeavyWeapon w, Player p) int
        +CalculateDamage(LightWeapon w, Player p) int
        +CalculateDamage(MagicWeapon w, Player p) int
        +CalculateDefense(Weapon w, Player p) int
    }

    IAttackStrategy <|.. NormalAttack : Atak zwykły
    IAttackStrategy <|.. StealthAttack : Atak skryty
    IAttackStrategy <|.. MagicAttack : Atak magiczny
    
    Player --> IAttackStrategy : Wybiera rodzaj ataku
    Player --> Enemy : Inicjuje walkę
    
    %% Double Dispatch representation (Visitor)
    IAttackStrategy ..> HeavyWeapon : Oblicza na podst. Siły
    IAttackStrategy ..> LightWeapon : Oblicza na podst. Zręczności
    IAttackStrategy ..> MagicWeapon : Oblicza na podst. Mądrości
```

<details>
<summary><strong>Stage 4: Configuration & Event Logging</strong></summary>
System initialization via external JSON/INI files. Implemented a thread-safe event log capturing all critical game state changes.
</details>

<details>
<summary><strong>Stages 5 & 7: AI Behavior & Acoustic Pathfinding</strong></summary>
Implemented noise propagation algorithms and collective NPC behaviors. Sound waves interact with the map topology, and NPCs communicate state changes within their respective species.
</details>

<details>
<summary><strong>Stage 6: Networked Multiplayer</strong></summary>
Migrated to an Authoritative Server model. Data synchronization is performed via `System.Text.Json` serialization.
</details>

```mermaid
classDiagram
    class Item {
        <<abstract>>
        +Symbol char*
        +Name string*
        +GetDamage() int
        +EquipLeft(Player p) void
        +EquipRight(Player p) void
        +UnEquip(Player p) void
        +Drop(Player p, Cell cell) void
        +PickUp(Player p, Cell cell) void
    }
    class Weapon {
        <<abstract>>
        +int damage
        +GetDamage() int
    }
    class OneHandedWeapon { <<abstract>> }
    class TwoHandedWeapon { <<abstract>> }
    class SlottedWeapon {
        -List~ISlotable~ slots
        +GetDamage() int
        +AddModifier(ISlotable component) void
    }
    class SocketAdapter {
        -List~ISlotable~ embeddedSlots
        +GetDamageModifier() int
        +GetStatModifier() int
    }
    class PassiveItem {
        +int statBonus
        +GetStatModifier() int
    }

    Item <|-- Weapon : Polymorphic Extension
    Weapon <|-- OneHandedWeapon
    Weapon <|-- TwoHandedWeapon
    Weapon <|-- SlottedWeapon
    SlottedWeapon "1" *-- "many" SocketAdapter : Composite Nesting
    SocketAdapter "1" *-- "many" PassiveItem : Holds Gemstones/Runes
    SocketAdapter "1" *-- "many" SocketAdapter : Recursive Mounting
```
## 🔧 Technical Setup

**System Requirements:**
* .NET SDK 8.0+

**Running the System:**

1. **Start the Server:**
   ```bash
   dotnet run -- --server 5555
2. **Connect a Client:**
   ```bash
   dotnet run -- --client 127.0.0.1:5555
---
Developed as a project for Object-Oriented Programming (Warsaw University of Technology).
