# Platformer

A side-scrolling 2D platformer in a melon-themed dungeon. Run, jump,
collect melons, dodge enemies, hit checkpoints to advance levels. Three
levels (`LevelOne` → `LevelTwo` → `Final`) with cross-scene state preserved.

## Controls

- **A / D** (or arrow keys) — move left / right (Unity Input Manager "Horizontal" axis)
- **Space** — jump (only when grounded)

## Scripts

| File | Role |
|---|---|
| `PlayerController.cs` | Movement, jump, sprite flip, animator wiring, ground-check overlap circle |
| `GameController.cs` | Lives, score, scene transitions, win/lose UI, singleton across scenes |
| `EnemyController.cs` | Subtracts a life on player contact; calls `Lose()` when lives hit 0 |
| `CheckpointBehaviour.cs` | Player-triggered, advances to next level (or final win screen) |
| `CollectibleController.cs` | "Melon" pickup — increments score, plays sound, destroys self |

## What was kept from the original CS 3350 assignment

- Multi-scene level progression via `SceneManager.LoadScene` — actual cross-level state survives the load thanks to the `DontDestroyOnLoad` singleton.
- Lives + Score split between cumulative (`Score`) and per-level (`_levelScore`) counters.
- Animator-driven sprite states (`isRunning`, `isGrounded`) — wired to a Unity Animator on Player.
- Tag-based collision routing (`Player` vs `Enemy` vs `Ground`) — the standard Unity 2D idiom.

## What the portfolio-polish pass fixed

- **`EnemyController` tag check was inverted.** Script lives on the enemy
  GameObject, so `OnTriggerEnter2D`'s `other` IS the player. The original
  checked `CompareTag("Enemy")` — which would only fire on enemy-vs-enemy
  collisions. Corrected to `CompareTag("Player")`.
- **Off-by-one on lives.** Original logic was `if (Lives > 0) UpdateLives() else Lose()`,
  which decremented past zero before triggering loss — player effectively
  got one extra free hit. Corrected: always decrement, then check
  post-value for `<= 0`.
- **`isGrounded` flipped true on walls too.** Original used
  `OnCollisionEnter2D` with tag check on "Ground"/"Platform" — touching
  a wall counted as grounded, enabling infinite wall-jumps. Replaced with
  a `Physics2D.OverlapCircle` at the player's feet, gizmo-visualized in
  the editor.
- **Multi-jump on held Space.** Original used `GetKey(Space)` so holding
  Space could queue multiple impulses on any 1-frame ground flicker.
  Changed to `GetKeyDown(Space)` — one press, one jump.
- **`Rigidbody2D.velocity` deprecated in Unity 6.3.** Migrated to
  `Rigidbody2D.linearVelocity` (4 read+write sites).
- **Singleton spawned duplicate GameControllers on every scene load.**
  `DontDestroyOnLoad(Instance)` was called in `Start` without an
  if-already-exists guard — so loading LevelTwo (which also has its own
  GameController in the scene) created a second GameController, then
  LoadScene("Final") created a third. Added an `Awake`-time singleton
  guard that destroys the duplicate before its Start runs.
- **Removed dead `using UnityEngine.SocialPlatforms.Impl;`** — IDE-added
  noise, unused.
- **Cached `AudioSource` reference** instead of repeated `Instance.GetComponent<AudioSource>()`
  per call.
- **`namespace Games.Platformer`** wrapped all five scripts so the
  `GameController`, `EnemyController`, etc. class names don't collide
  with other games in the umbrella project.

## What's intentionally left as-is

- No coyote time / jump buffer — modern platformers are forgiving, but
  this is a coursework rebuild, not a Celeste competitor. Kept the raw
  feel.
- Score / `_levelScore` split — original design choice. Score persists
  across levels for the final tally; `_levelScore` shows per-level progress.
- Tag-based ground identification at the layer level — could be replaced
  with a `Layer` mask only, but tag-based reads more obvious.

## Open in Unity

From the project root: Unity Hub → `unity-game-collection` → load
`Assets/_Games/Platformer/Scenes/LevelOne.unity`. The other levels are
loaded by `GameController.GoToNextLevel()` via scene name — make sure
all three are added to the build's **Scene List** before testing the full
progression. In Unity 6: **File → Build Profiles** (`Ctrl+Shift+B`) →
pick your active profile → Scene List → drag the three scene files in,
or "Add Open Scenes" with each open. First scene in the list is the
startup scene. (Unity 6 renamed the old "Build Settings" menu to
"Build Profiles" and split per-platform scene lists out from a global list.)

## Setup checklist (Unity-side, won't compile-fail but won't play right)

1. **Player tag**: select Player GameObject → Inspector → Tag → **Player**
2. **Enemy/Ground tags**: ensure enemy prefabs use "Enemy" tag, ground/platform tiles use "Ground" or "Platform" tag
3. **Ground check**: PlayerController now needs a `_groundCheck` Transform. Create an empty child of Player at the feet position, drag into the GroundCheck slot, set Ground Layers mask to whatever layer your ground/platforms sit on
4. **Scene List**: File → Build Profiles (`Ctrl+Shift+B`) → drag LevelOne, LevelTwo, Final from `Scenes/` folder into the active profile's Scene List
