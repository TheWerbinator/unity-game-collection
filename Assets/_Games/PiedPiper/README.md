# Pied Piper

A medieval take on Snake. You play the Piper; "children" idle around the
map and you collect them by walking near. Each collected child joins a
chain that follows the path you've walked. Running into your own chain
ends the game. Score decays over time; collecting children adds to it.

**Planned additions** (in progress, see polish list below):
- City guards that patrol — touching one ends the game
- Quota of children to collect before an exit gate activates
- Win condition: lead the chain through the exit

## Controls

- **W / A / S / D** — move (now supports diagonals)
- Game starts on first directional keypress

## Rules

- **Don't stop.** Past the third follower, the chain re-enables its collider after 2 s and catches up to you if you idle. Movement keeps you alive — same loop as Snake.
- **Don't hit a wall.** Out-of-bounds at ±20 × ±10 (medieval square edges) = instant lose.
- **Score decays over time.** +5 per child collected; ticks down by Time.deltaTime per second while alive.

## Scripts

| File | Role |
|---|---|
| `PiperBehaviour.cs` | Movement, child pickup, score decay, lose-on-chain-hit |
| `ChildBehaviour.cs` | Idle spin → follow path → re-enable collider after delay |
| `PathRecorder.cs` | Per-entity path queue (Piper + each child) |
| `GameController.cs` | Spawn cadence, loss handling, domino-death coroutine |

## What was kept from the original CS 3350 assignment

- Per-entity `PathRecorder` so each child plays back the path of the
  child in front of it — the chain propagates organically without a
  central path manager.
- Score-decays-over-time pressure (Pied Piper is a vampire that pays you
  by the second).
- Rigidbody velocity for movement so the player feels physical, not grid-snapped.
- Original art: free houses, cobblestone, chess-piece characters,
  fence, and royalty-free music from fesliyanstudios.

## What the portfolio-polish pass fixed

- **Spawn timer was inverted.** Original code spawned children when
  `ChildSpawnTimer > 0` — i.e. immediately on the first tick before the
  timer had a chance to count down. Fixed to `<= 0` so the timer actually
  acts as a delay.
- **`Rigidbody.velocity` deprecated in Unity 6.3.** Migrated to
  `Rigidbody.linearVelocity`, the new name.
- **Broken loss coroutine.** `WaitAndDestroy(child, interval)` was called
  as a method (not via `StartCoroutine`), so the yield never ran and all
  children vanished at once. Rewritten as a proper coroutine
  (`ProcessLoss`) that destroys children sequentially with a shrinking
  delay — the intended "domino death" effect now actually happens.
- **`_followOffset = -50f` was nonsense.** First follower spawned 50 units
  *behind* the Piper, way off the map. Set to a sensible 1.5 units.
- **PathRecorder ran at 1000 Hz.** `recordInterval = 0.001f` queued
  ~1000 positions per second per recorder — memory bloat on long sessions.
  20 Hz (0.05f) is plenty smooth for the follower's `MoveTowards`
  interpolation and 50× less heap churn.
- **No diagonal movement.** The original used an `if / else if` chain
  that picked only one direction per frame even when two keys were
  held. Replaced with a normalized input vector that supports diagonals
  at uniform speed.
- **Dead code removed**: unused `_wallObject` field, redundant
  `targetChild` variable in collision handling, redundant `Following`
  re-check after the early return.
- **`namespace Games.PiedPiper`** wrapped all four scripts so the next
  game's `GameController` doesn't clash with this one.

## What's intentionally left as-is

- **`MovementSpeed *= 1.01f` per child collected.** Snake-style speed
  creep is the intended difficulty ramp.
- **Hardcoded bounds (±20 x, ±10 z).** Matches the scene's medieval
  square layout; not worth config-ifying for a single-scene game.
- **Children 0, 1, 2 stay collider-disabled.** Closest followers can't
  bite the Piper — protects against insta-loss when you turn around
  immediately after picking one up.

## Open in Unity

From the project root, open Unity Hub → `unity-game-collection`, then
load `Assets/_Games/PiedPiper/Scenes/PiedPiper.unity`.
