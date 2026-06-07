# unity-game-collection

Five small games from **CS 3350 (Game Design & Development)** at Southern
Utah University, polished and bundled into a **single Unity 6.3 LTS
project** with one shared WebGL build on itch.io. Originals were Unity
2022.3 LTS coursework; the portfolio rewrite migrated them onto current
LTS.

Game dev isn't my career direction; this exists to show I can ship a
working game loop end-to-end (input, state, collision, win/lose, UI) and
that I followed a multi-game semester through to completion.

**▶ [Play in browser on itch.io](https://thewerbinator.itch.io/unity-game-collection)**

Click the game canvas once to give it keyboard focus, then use the
MainMenu buttons. Esc returns to the menu from any game.

## Status

| Game | Status | Note |
|---|---|---|
| Breakout | ✅ landed | Hand-rolled AABB, paddle-relative bounce; polish pass fixed precedence bug + moved physics to FixedUpdate |
| Pied Piper | ✅ landed | Medieval take on Snake — quota of children → exit gate activates; chain catches up if you stop; scatter-on-loss. Guards + extra levels deferred. |
| Platformer | ✅ landed | 2D side-scroller, 3 levels (LevelOne → LevelTwo → Final), melon collectibles, enemies + lives. Polish fixed tag-inversion, off-by-one lives, wall-jump exploit, multi-jump bug, singleton duplicate, HUD anchoring. |
| FPS | ✅ landed | First-person with key-card doors + patrolling enemies. 11 scripts, OOP inheritance + composition (Character/Weapon base classes). Polish fixed miss-fire null-ref, raycast-magnitude bug, projectile leak, deprecated velocity API. More enemies + key-HUD + raycast layer-masking deferred. |
| DOOM Clone | ✅ landed | Doom homage — ProBuilder-built E1M1 level, NavMesh AI, billboard sprites, vertical-autoaim hitscan, Doom-style armor absorption. Assets swapped to Freedoom (GPL) + royalty-free metal. Pickups + full STBAR HUD + level progression deferred. |

## Architecture

One Unity project. Each game lives under `Assets/_Games/<GameName>/` with
its own scenes, scripts, prefabs, and materials. Per-game scripts are
wrapped in `namespace Games.<GameName>` so cross-game class collisions
(every game has a `GameManager`) don't happen. `Assets/_Shared/` holds the
`MainMenu` scene and the `SceneLoader` singleton that drives menu
transitions + the Esc-to-menu hotkey.

```
Assets/
├── _Games/
│   ├── Breakout/
│   │   ├── Scenes/Breakout.unity
│   │   ├── Scripts/{BallBehaviour, GameManager, PaddleController}.cs
│   │   ├── Prefabs/
│   │   └── README.md
│   ├── PiedPiper/                  ← Pied Piper variant of Snake
│   ├── Platformer/                 ← 2D side-scroller, 3 levels
│   ├── FPS/                        ← First-person with keys + doors + enemies
│   └── DoomClone/                  ← Doom homage: ProBuilder + NavMesh + Freedoom sprites
├── _Shared/
│   ├── Scenes/MainMenu.unity
│   └── Scripts/SceneLoader.cs   ← singleton, drives menu transitions + Esc-to-menu
├── Settings/         ← URP, input
└── TextMesh Pro/
```

## Why one project not five

- Single WebGL build, one itch.io page, one click to play any of them
- Shared URP + TextMesh Pro + render settings = no per-game configuration drift
- Demonstrates Unity scene-loading + namespace hygiene across multiple game loops in one assembly

## Per-game READMEs

Each game's folder has its own README covering: original design choices,
what the polish pass changed, controls, and how to open the scene.

- [Breakout](Assets/_Games/Breakout/README.md)
- [Pied Piper](Assets/_Games/PiedPiper/README.md)
- [Platformer](Assets/_Games/Platformer/README.md)
- [FPS](Assets/_Games/FPS/README.md)
- [DOOM Clone](Assets/_Games/DoomClone/README.md)

## Stack

- Unity 6.3 LTS
- Universal Render Pipeline (latest with Unity 6.3)
- C# (Mono / .NET Standard 2.1 — Unity's runtime)
- WebGL build target → itch.io

## License

MIT — see [LICENSE](LICENSE).
