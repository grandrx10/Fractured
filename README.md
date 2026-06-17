# Fractured

A 3D action-adventure game built in Unity where everything is a **card**. Cards aren't just menu items — they're throwable, usable objects that drive combat, puzzles, and world interaction through a flexible, composition-based behavior system.

## 🎥 Demo

[**Watch the gameplay demo on YouTube**](https://youtu.be/iiJcUfR9C-g?si=3MN_gj0FX-C6QRr_)

## 📸 Screenshots

> _Drop screenshots into a `docs/screenshots/` folder and update the paths below._

| | |
| --- | --- |
| ![Screenshot 1](docs/screenshots/screenshot-1.png) | ![Screenshot 2](docs/screenshots/screenshot-2.png) |
| ![Screenshot 3](docs/screenshots/screenshot-3.png) | ![Screenshot 4](docs/screenshots/screenshot-4.png) |

<!--
Add more shots as needed, e.g.:
![Boss fight](docs/screenshots/boss-fight.png)
![Puzzle minigame](docs/screenshots/puzzle.png)
-->

## Overview

In *Card*, the player is an agent that owns a deck and a hand of cards. Cards carry **Behaviors** that define how they interact with the world — throwing, healing, attacking, triggering effects, and more. The same card framework powers:

- **Combat** against bosses and minions in dedicated arenas
- **Puzzle minigames** (fishing, cooking, pipe puzzles, reflectors, pressure plates, weight pulleys, and more)
- **Open-world exploration** across seasonal domains (Spring, Fall, Sun, etc.) with NPCs and dialogue

## Core Concepts

The game is built around a small set of composable abstractions. (Full architecture documentation lives in [`Assets/ReadMe.md`](Assets/ReadMe.md).)

| Concept | Role |
| --- | --- |
| **Behavior** | A `ScriptableObject` defining a single card interaction. Cards can hold any number of behaviors. Supports both composition and inheritance via **Behavior Tags** (interfaces). |
| **Card** | A container for Behaviors with utility functions, UI, and state. Supports default behaviors (e.g. health, use) applied automatically at initialization. |
| **Agent** | A container for Cards. Manages a **hand** (active cards) and a **deck** (owned cards), and provides cards to environments on request. |
| **Environment** | A system that interacts with Agents and manages temporary state (health, mana) and its associated UI. **Physical Environments** add physics-driven interactions like card throwing. |

### Key systems

- **Behavior Tags** — interfaces that cards filter by, enabling clean polymorphic interactions.
- **Default Behaviors** — fallback behaviors substituted at init when no card behavior implements a given type.
- **Async card selection** — `SelectCardAsync` / `SelectCardImmediate` let environments request cards from agents.
- **GlobalState** — a singleton storing game state as Events, Strings, Integers, and Tuples.
- **Utilities** — `Delay`, `CallbackWaiter`, `ArticleHelper`, `FloatBar` UI, and a custom `[PrefabComponent]` editor attribute.

## Tech Stack

- **Engine:** Unity `6000.0.33f1` (Unity 6)
- **Render pipeline:** Universal Render Pipeline (URP) 17.0.3 with volumetric fog
- **Input:** Unity Input System
- **Notable packages:** Cinemachine, AI Navigation, Animation Rigging, ProBuilder, Polybrush, Terrain Tools, Visual Effect Graph, Timeline
- **Third-party:** [OpenFracture](https://github.com/dgreenheck/OpenFracture), [Noisy Nodes](https://github.com/JimmyCushnie/Noisy-Nodes), [URP Volumetric Fog](https://github.com/mseonKim/URP-VolumetricFog-ForwardPlus), LeanTween

## Project Structure

```
Assets/
├── Cards/          Card framework — Agent, Behaviors, Core tags, Environments, visuals
├── Game/           Runtime systems — combat env, bosses, minions, health, shop, audio, VFX
├── Minigames/      Self-contained puzzles (Fishing, Cooking, PipePuzzle, Reflector, ...)
├── Characters/     Player & NPC models, animations, dialogue, interactables
├── World/          Environments, seasonal houses/towns, terrain, shaders, subworlds
├── Scenes/         Test and gameplay scenes (boss tests, card tests, seasonal levels)
├── Player/         Player-specific assets
├── Material/       Shared materials
└── ReadMe.md       In-depth code/architecture documentation
```

## Getting Started

1. Install **Unity 6 (`6000.0.33f1`)** via Unity Hub.
2. Clone this repository.
3. Open the project folder in Unity Hub — packages (including Git-based dependencies) resolve automatically on first open.
4. Open a scene from `Assets/Scenes/` to start exploring:
   - `Card_Test_1` — card mechanics sandbox
   - `Boss_Test_1` / `Boss_Test_2` / `Boss_Test_3` — boss encounters
   - `SpringMayor`, `FallWolf`, `GoatsAndCrows`, etc. — world & narrative levels
5. Press **Play**.

## Documentation

For implementation details — how to author new Behaviors, define defaults, build physical environments, and use the utility classes — see [`Assets/ReadMe.md`](Assets/ReadMe.md).
