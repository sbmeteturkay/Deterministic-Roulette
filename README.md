## Deterministic Roulette Case Study

### How to play

Clone the repository. Open with Unity 2022.3.62f1. Find the main scene from **Assets->Project->Scenes->Gameplay** and hit play. Select bet from bet selection ui and place on roulette desk. Press spin to spin wheel. You cant spin if bet more than your balance so clear bets if you cant spin.


### Demo Play

![Uygulama Ekran Görüntüsü](https://via.placeholder.com/468x300?text=App+Screenshot+Here)


### Features

- Custom physics including roulette ball, wheel, deflectors
- Supports standard Inside Bets (Straight, Split, Street, Corner, Six Line) and Outside Bets (Red/Black, Even/Odd, High/Low, Dozens, Columns).
- Deterministic mode for selecting next number
- Historical record/player stats
- Save system


## Technical Details
### 🎮 GameManager Initialization Flow
The core architecture is initialized inside the GameManager.Awake() method. The setup follows a modular and loosely coupled design that applies several OOP principles such as Encapsulation, Abstraction, and basic Dependency Injection (manual).

## 🧠 Architecture & Core Modules

### 📦 Modular System Design

The project follows a **feature-based modular structure**, composed of:

- **Feature Modules**: Isolated gameplay packages like `BettingSystem`, `RoulettePhysics`, and `PlayerStats`, each with their own `.asmdef` files.
- **Core Module (`Core/`)**: Central system initialization and high-level orchestration logic (e.g., `GameManager`).
- **Shared Layer (`Scripts/Shared/`)**: Acts as a communication and utility layer between features and core, holding interfaces, shared services, and ScriptableObjects.

---

### 🧩 Feature Communication via Shared Layer

Feature modules **do not depend on each other directly**. Instead, interaction is mediated via shared interfaces and services registered in the `Shared` layer.

**Key shared elements:**

- `ServiceLocator`: Provides access to cross-feature services like audio, allowing decoupled access to dependencies.
- Interfaces for communication between feature packages.
- Shared `ScriptableObject` assets defining configuration or view models.

---

### 🎰 RoulettePhysics Module (MVC Pattern)

The `RoulettePhysics` module is structured using the **Model-View-Controller (MVC)** pattern:

- **Model**: Backed by a `ScriptableObject`, containing wheel configuration and physical settings for the roulette table.
- **View**: Visual wheel and ball, receives data from controller.
- **Controller**: Handles deterministic or random spin outcomes and passes results back to betting logic.

This structure allows easy **table configuration changes** by editing the model asset, without code changes.

---

### 🎯 BettingSystem & AreaSelector Integration

The **BettingSystem** relies on a dedicated `AreaSelector` component to define valid bet regions:

- Bet areas are defined via a **2D layout** in a `ScriptableObject`.
- These areas include **signal identifiers** for each region (e.g., `"Straight_13"`, `"Column_2"`).
- When the user interacts with the table, `AreaSelector` forwards the selected signal to `BettingSystem`, which resolves it into a valid `Bet`.

✅ This design allows roulette table layouts to be modified or extended easily by editing the area definition asset.

The following signals represent different types of bets on the roulette table:

- `"0"` – `"36"` → Straight bet (single number)
- `"112"` / `"212"` / `"312"` → Dozen bets (1st / 2nd / 3rd dozen)
- `"118"` → Low bet (1–18)
- `"1936"` → High bet (19–36)
- `"134"` / `"235"` / `"336"` → Column bets (1st / 2nd / 3rd column)
- `"even"` → Even numbers
- `"odd"` → Odd numbers
- `"red"` → Red numbers
- `"black"` → Black numbers


---

### 🔊 Audio System via ServiceLocator

The audio system is centralized and accessible via the `ServiceLocator`:

- **`SoundManager`**: Registered by `GameManager` at runtime, provides audio playback services.
- **`SoundLibrary` (ScriptableObject)**: A configurable asset holding references to SFX and music clips.
- **Access Pattern**: Any system can request a sound using a string key:

```csharp
ServiceLocator.SoundService.Play("ChipPlace");
```
## 🔄 OOP Principles in the Project

This project applies core Object-Oriented Programming principles throughout its architecture to ensure modularity, maintainability, and extensibility.

### ⚙️ Encapsulation

- Internal data is kept private inside classes such as `PlayerStatsModel`, `ChipManager`, and `BettingSystem`.
- Public methods are used to interact with these systems, ensuring controlled access and hiding implementation details.

### 🧬 Abstraction

- Shared interfaces (e.g., audio, betting area selection) are defined under `Scripts/Shared/` and consumed by core or feature modules.
- Consumers do not rely on concrete implementations, enabling decoupling between systems.

### 🧰 Inheritance & Polymorphism

- Various `Bet` types (e.g., StraightBet, ColumnBet) likely inherit from a common base or implement a shared interface to enable polymorphic handling within the `BettingSystem`.
- This allows for generalized processing of diverse bet types using a unified API.

### 📦 SOLID Principles

- **S — Single Responsibility**:  
  Each class has a clear and isolated responsibility.  
  E.g., `SoundManager` only handles audio playback; `BettingSystem` only handles bet logic.

- **O — Open/Closed**:  
  New bet types or roulette configurations can be added without modifying existing classes.  
  This is enabled by polymorphism and external configuration (via ScriptableObjects).

- **L — Liskov Substitution**:  
  Any subclass or implementation (e.g., a new bet type) can be used interchangeably where the base interface is expected.

- **I — Interface Segregation**:  
  Shared interfaces are lean and specific to features (e.g., area selection or chip usage), preventing bloated abstractions.

- **D — Dependency Inversion**:  
  High-level modules depend on abstractions (interfaces in `Shared/`), not concrete implementations.  
  Dependencies (like `SoundManager`) are provided via a `ServiceLocator`, further decoupling instantiation from usage.

---

Together, these principles form the foundation of a clean, scalable, and testable codebase.
