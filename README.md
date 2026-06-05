# unity-game-collection

Five small games from **CS 3350 (Game Design & Development)** at Southern
Utah University, polished and bundled into a **single Unity 6.3 LTS
project** with one shared WebGL build on itch.io. Originals were Unity
2022.3 LTS coursework; the portfolio rewrite migrated them onto current
LTS.

Game dev isn't my career direction; this exists to show I can ship a
working game loop end-to-end (input, state, collision, win/lose, UI) and
that I followed a multi-game semester through to completion.

🌐 Live build (coming after all five land): _itch.io link TBD_

🚧 Work in progress — adding games one at a time after a per-game polish pass.

## Status

| Game | Status | Note |
|---|---|---|
| Breakout | ✅ landed | Hand-rolled AABB, paddle-relative bounce; polish pass fixed precedence bug + moved physics to FixedUpdate |
| Pied Piper | 🚧 in progress | Medieval take on Snake — chain of children follows your path; adding city guards to dodge + collect-N-children quota before exit activates |
| Platformer | pending | — |
| FPS | pending | — |
| DOOM Clone | pending | — |

## Architecture

One Unity project. Each game lives under `Assets/_Games/<GameName>/` with
its own scenes, scripts, prefabs, and materials. Per-game scripts are
wrapped in `namespace Games.<GameName>` so cross-game class collisions
(every game has a `GameManager`) don't happen. A `MainMenu` scene + scene
loader will land in `Assets/_Shared/` once all five games are in.

```
Assets/
├── _Games/
│   ├── Breakout/
│   │   ├── Scenes/Breakout.unity
│   │   ├── Scripts/{BallBehaviour, GameManager, PaddleController}.cs
│   │   ├── Prefabs/
│   │   └── README.md
│   ├── PiedPiper/                  ← Pied Piper variant of Snake (in progress)
│   └── (Platformer, FPS, DoomClone — pending)
├── _Shared/          ← MainMenu + SceneLoader (pending)
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

## Stack

- Unity 6.3 LTS
- Universal Render Pipeline (latest with Unity 6.3)
- C# (Mono / .NET Standard 2.1 — Unity's runtime)
- WebGL build target → itch.io

## License

MIT — see [LICENSE](LICENSE).
