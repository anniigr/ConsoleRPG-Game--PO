<p align="center">
  <img src="https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjEx.../giphy.gif" width="500px" alt="Gameplay preview"/>
</p>

# ⚔️ Console RPG

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/Console-000000?style=for-the-badge&logo=windows-terminal&logoColor=white" alt="Console"/>
</p>

## 📘 Оглавление

1. [📖 Обзор](#-обзор)
2. [🎯 Возможности (Особенности)](#-возможности)
3. [🏗️ Архитектура](#-архитектура)
4. [🎮 Управление](#-управление)

## 📖 Обзор

**Console RPG** — это классическая ролевая игра, работающая прямо в терминале Windows. Проект демонстрирует принципы объектно-ориентированного программирования (ООП), инкапсуляции и чистого кода на языке C#. 

В игре реализована система перемещения по карте, генерация окружения (стены и пол), а также полноценная система экипировки и инвентаря. Вы можете исследовать мир, находить золото, подбирать оружие и брать его в разные руки для расчета суммарного урона! 🛡️🗡️

## 🎯 Возможности

> 👇 **Нажмите на любую из возможностей ниже, чтобы узнать детали**

<details>
<summary>
🗺️ <strong> Исследование мира </strong> - Перемещайтесь по сетке карты, избегая препятствий
</summary>
<br>
<ul>
  <li>&nbsp Карта имеет размеры 40x20 клеток.</li>
  <li>&nbsp Система коллизий не позволяет проходить сквозь стены (<code>Wall</code>).</li>
  <li>&nbsp На полу (<code>Floor</code>) можно находить спрятанные предметы.</li>
</ul>
</details>

<details>
<summary>
🎒 <strong> Продвинутый инвентарь </strong> - Собирайте и управляйте своими вещами
</summary>
<br>
<ul>
  <li>&nbsp Поднимайте предметы с пола нажатием одной кнопки.</li>
  <li>&nbsp Выбрасывайте ненужный мусор (например, кости или старые кружки) обратно на карту.</li>
  <li>&nbsp Экономика: подбирайте монеты и слитки золота, которые автоматически попадают в ваш кошелек.</li>
</ul>
</details>

<details>
<summary>
⚔️ <strong> Боевая экипировка </strong> - Настраивайте свое оружие
</summary>
<br>
<ul>
  <li>&nbsp <strong>Одноручное оружие:</strong> Мечи и топоры можно взять как в левую, так и в правую руку.</li>
  <li>&nbsp <strong>Двуручное оружие:</strong> Тяжелые мечи (Greatswords) занимают сразу обе руки.</li>
  <li>&nbsp Автоматический подсчет общего урона персонажа в зависимости от экипировки.</li>
</ul>
</details>

<details>
<summary>
📊 <strong> Экран статистики </strong> - Отслеживайте состояние персонажа
</summary>
<br>
<p>
В реальном времени на экране отображаются ваши характеристики: HP, Сила, Ловкость, Удача, а также текущее состояние левой и правой руки. Журнал событий (LOG) внизу экрана подскажет, что произошло на последнем ходу.
</p>
</details>

## 🎮 Управление

Игра имеет два режима: **Режим Карты** и **Режим Экипировки**.

### На карте:
* `W`, `A`, `S`, `D` — Перемещение персонажа.
* `E` — Поднять предмет с пола.
* `I` — Открыть инвентарь.
* `Q` или `Esc` — Выход из игры.

### В инвентаре:
* `W`, `S` — Выбор предмета (курсор).
* `L` — Взять предмет в левую руку.
* `R` — Взять предмет в правую руку.
* `1` / `2` — Снять экипировку с левой / правой руки.
* `Q` — Выбросить предмет на пол.
* `I` или `Esc` — Закрыть инвентарь.

## 🏗️ Архитектура

Проект четко разделен на логические пространства имен (Namespaces):

* **`ConsoleRPG.World`** — Логика карты (Map, Cell) и типов местности (Terrain, Wall, Floor).
* **`ConsoleRPG.Entities`** — Сущности игры (включая класс Player с его характеристиками и логикой перемещения).
* **`ConsoleRPG.Items`** — Базовые классы предметов и реализация (оружие, валюта, мусор).
* **`ConsoleRPG.Engine`** — Ядро игры. `GameEngine` обрабатывает ввод, а `Renderer` отвечает за покадровую отрисовку интерфейса с помощью `StringBuilder`.

### Диаграмма классов предметов (Items)
Ниже представлена структура наследования игровых предметов:

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
