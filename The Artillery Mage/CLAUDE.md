# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**The Artillery Mage** is a Unity 3D tower defense / spell-casting game. The player casts spells via key sequences to defend locations from demon invasion. Built with Unity 2022.3.62f2 LTS and Universal Render Pipeline (URP).

## Build & Play

- **Required**: Unity 2022.3.62f2 LTS (exact version — use Unity Hub)
- **Play**: Open project in Unity Editor → Press Play (no CLI build step)
- **Main scene**: `Assets/Scenes/SampleScene.unity`
- **VFX dev scene**: `Assets/Scenes/VFX Testing.unity`
- **Standalone build**: File > Build Settings in Unity Editor

## Architecture

### Core Pattern: Singleton Managers + Events

All core systems are singletons (`static Instance`) that communicate via C# events/delegates rather than direct references. Scripts subscribe to events from managers they depend on.

```
GameManager ← RequestManager.OnRequestPassed
SpellCaster → OnCastedSpell → RequestManager (validates spell against active requests)
MovementSystem → OnArrived(locationId)
```

### Key Systems (`Assets/Systems/`)

| System | File | Responsibility |
|---|---|---|
| `GameManager` | `Systems/GameManager.cs` | Mana (drains 0.5/sec), unit counts per location, demon expansion (every 10s), win/lose |
| `MovementSystem` | `Systems/MovementSystem.cs` | Camera movement between locations, DOTween transitions, screen fades |
| `RequestManager` | `Systems/REquest_N_Spells/RequestManager.cs` | Spawns/tracks quest requests (20–35s window), validates conditions via `MathEquation` |
| `SpellCaster` | `Systems/REquest_N_Spells/SpellCaster.cs` | Detects key sequences, 2s input timeout, 1s cast delay, instantiates VFX |

**Win/lose**: Castle falls below 20% good units → game over.

### Data: ScriptableObjects

New content is added by creating ScriptableObject assets in `Assets/GameData/`:

- **`SpellData`**: Key sequence (`List<KeyCode> inputs`) + VFX prefab. WASD aliases arrow keys.
- **`RequestData`**: Target location, spawn condition (`MathEquation percentGoodEquation`), and `List<Outcome>` mapping spells to unit changes.
- **`LocationList` enum**: All valid locations (`CropsArea`, `Castle`, `Village`, `Forest`, `DemonGate`, `Volcano`, …).

Spell and request pools on their respective managers auto-populate in the editor via `[Button]`.

### Utilities

- **`Systems/Helper Functions/Extensions.cs`**: `GetRandom<T>()`, `MeetsEquation()`, vector plane helpers (`XOZ`, `OYZ`, `XYO`), `DestroyAllChildren()`, `FormatNumber()` (ordinals)
- **`SerializableDictionary/SerializableDictionary.cs`**: Generic Inspector-serializable dictionary — used for KeyCode→sprite maps, location→transform maps, etc.

## Plugins

- **DOTween** — all tweened animations and transitions
- **Odin Inspector** — `[Button]`, `[ReadOnly]`, `[TextArea]` attributes; required for SerializableDictionary inspector support
- **TextMeshPro** — all UI text
- **URP + Visual Effect Graph** — rendering and spell VFX
