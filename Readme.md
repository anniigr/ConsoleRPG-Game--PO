<p align="center">
  <img src="https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjEx.../giphy.gif" width="600px" alt="Multiplayer Console RPG Gameplay"/>
</p>

# ⚔️ Console RPG: TCP Multiplayer & MVC Architecture

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/Architecture-MVC-blue?style=for-the-badge" alt="Architecture: MVC"/>
  <img src="https://img.shields.io/badge/Networking-TCP%20%2F%20JSON-orange?style=for-the-badge" alt="TCP Network"/>
  <img src="https://img.shields.io/badge/Clean%20Code-No%20RTTI-success?style=for-the-badge" alt="Clean Code"/>
</p>

## 📖 Overview

This project is not just a standard console game; it is a **comprehensive architectural challenge** developed over 7 rigorous iterations. Built entirely in C#, the application evolved from a simple single-player grid explorer into a **fully synchronized, thread-safe, multiplayer network game** supporting up to 9 concurrent players.

The core engineering constraint of this project was absolute adherence to Object-Oriented Programming (OOP) principles. The codebase strictly avoids Run-Time Type Information (RTTI) such as `is`, `as`, `typeof`, and type-identifying `enums`. Instead, all logic is driven by clean polymorphism and classical Gang of Four (GoF) design patterns.

## 🏗️ Architectural Highlights

To satisfy the demands of real-time multiplayer synchronization and complex game logic, the project utilizes an **Authoritative Server** model wrapped in an **MVC (Model-View-Controller)** architecture:

* **Model (Source of Truth):** Encapsulates the entire game state (map grid, player stats, NPC behavior, item locations). It is entirely decoupled from the console and network layers.
* **View:** Responsible strictly for rendering data. In client mode, it parses incoming JSON state payloads and efficiently draws them to the console.
* **Controller:** Captures, validates, and routes player inputs (W, S, A, D, E, J) to the Model.
* **Concurrency & Networking:** Custom implementation using `TcpListener` and `TcpClient`. Thread safety is ensured via `lock` mechanisms and thread-safe collections to handle simultaneous TCP requests without race conditions.

---

## 🧩 Applied Design Patterns

A major focus of this project was applying the right design patterns to solve complex scaling issues without hardcoding logic or creating massive `if/switch` statements.

| Design Pattern | Implementation in Project | Solved Engineering Problem |
| :--- | :--- | :--- |
| **Observer** | Sound Propagation & Herd AI | Allowed enemies to react to noise and intra-species deaths without tight coupling. Implemented *without* the C# `event` keyword to demonstrate deep OOP understanding. |
| **Composite** | Slotted Items & Sockets | Handled complex inventory math recursively (e.g., calculating stats for a sword holding a socket, which holds another socket and a magic stone). |
| **Decorator** | Dynamic Weapon Modifiers | Applied infinite stackable modifiers (e.g., "Unlucky", "Strong") to weapons during world generation without modifying base classes. |
| **Strategy / Builder** | Procedural Dungeon Generation | Enabled modular creation of dungeons based on specific configuration "Themes" (e.g., corridors, rooms, artifact placements). |
| **State** | Reactive Enemy AI | Allowed NPCs to dynamically switch behaviors (Follow, Flee, Ignore) based on a Sight > Sound priority hierarchy and HP thresholds. |

---

## 🚀 Key Features by Development Stage

> 👇 **Click to expand the technical details of each development phase**

<details>
<summary><strong>🗺️ Stage 1 & 2: Procedural Generation & Grid Engine</strong></summary>
<br>
<ul>
  <li>Constructed a 20x40 coordinate grid system with solid collision detection.</li>
  <li>Implemented procedural dungeon building sequences (empty halls, corridors, central rooms, randomized loot).</li>
  <li>Developed a dynamic, context-aware UI that adapts its instructions based on the specific elements generated in the current dungeon.</li>
</ul>
</details>

<details>
<summary><strong>⚔️ Stage 3: Polymorphic Combat System</strong></summary>
<br>
<ul>
  <li>Turn-based combat calculating armor mitigation, base attack, and HP.</li>
  <li>3 Weapon Categories (Heavy, Light, Magic) and 3 Attack Styles (Normal, Stealth, Magic).</li>
  <li>Damage and defense scaling is handled entirely via polymorphism, completely eliminating "spaghetti code" <code>switch</code> statements for combat resolution.</li>
</ul>
</details>

<details>
<summary><strong>📜 Stage 4: Event Sourcing & JSON Configurations</strong></summary>
<br>
<ul>
  <li>Game initialization via external <code>.ini</code> / <code>.json</code> config files determining Player Name, Dungeon Theme, and Log paths.</li>
  <li>Dungeon Themes control item pools, specific artifact generation, and enemy types (e.g., "Vault" theme spawns aggressive safes and gold).</li>
  <li>Thread-safe Event Log system writing real-time combat and exploration data to custom timestamped files.</li>
</ul>
</details>

<details>
<summary><strong>🧠 Stage 5 & 7: Advanced AI & Wave Propagation</strong></summary>
<br>
<ul>
  <li><strong>Acoustic Pathfinding:</strong> Sound generated by dropped/equipped items travels through corridors (blocked by walls). Heavy weapons generate high-range noise, light weapons are stealthy.</li>
  <li><strong>Herd Mechanics:</strong> Enemies belong to species (e.g., Goblins, Skeletons). Killing an aggressive skeleton buffs the remaining pack, while killing a cowardly goblin nerfs them.</li>
  <li><strong>Behavior Trees:</strong> NPCs dynamically follow or flee from players based on line-of-sight checks and acoustic pings.</li>
</ul>
</details>

<details>
<summary><strong>🌐 Stage 6: TCP Multiplayer Backend</strong></summary>
<br>
<ul>
  <li>Refactored the entire codebase to support up to 9 players sharing the same map.</li>
  <li>Command-line arguments allow the app to boot as an Authoritative Server (<code>--server [port]</code>) or a Remote Client (<code>--client [ip:port]</code>).</li>
  <li>State broadcasting using <code>System.Text.Json</code> for object serialization and Data Transfer Objects (DTOs).</li>
  <li>Handled network latency and simultaneous turn-queuing using multithreading.</li>
</ul>
</details>

---

## 🎮 How to Run

You can run this project locally to test the multiplayer capabilities. 

**1. Clone the repository:**
```bash
git clone [https://github.com/YourUsername/ConsoleRPG-MVC-Multiplayer.git](https://github.com/YourUsername/ConsoleRPG-MVC-Multiplayer.git)
cd ConsoleRPG-MVC-Multiplayer
