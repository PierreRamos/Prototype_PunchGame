# Pixel Punch

A fast 2D brawler prototype in the style of *One Finger Death Punch*: enemies rush in from both sides and every input
counts. Built solo in Unity. All code and pixel art by me.

▶️ **Download:** [pierrymarbles.itch.io/punch-game](https://pierrymarbles.itch.io/punch-game)

<!-- TODO: add 2–3 screenshots or a short gameplay GIF here, e.g. ![Gameplay](docs/gameplay.gif) -->

## Features
- **Enemy variety:** multiple enemy types, including elites with their own mechanics and hit-type indicators
- **Hold battles:** a separate one-on-one encounter mode with its own timer and prompts
- **Player kit:** attacks, dash, and a special meter with its own display
- **Difficulty scaling:** a dedicated difficulty manager driving enemy spawns
- **Game feel:** camera shake, camera effects, hit indicators and pooled particle effects

## Technical highlights
- **Object pooling** for enemies and particle effects, so heavy fights don't spike garbage collection
- **Event-driven architecture** (`System_EventHandler`) that keeps player, enemy and UI systems decoupled
- **Systems split by responsibility:** player, enemy, and game-manager scripts in separate folders

## Project layout
```
PunchGame/Assets/Scripts/
├── Enemy/          # enemy types, elites, hit handling, drops
├── Game Manager/   # spawning, pooling, difficulty, audio, UI, time
└── Player/         # controller, movement, attack, health, status
```

## Built with
Unity · C# · Aseprite
