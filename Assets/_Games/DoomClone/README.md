# DOOM Clone

A small first-person shooter modeled after id's 1993 Doom. ProBuilder-built
level (`E1M1.unity` — direct Doom level-naming reference), NavMesh-driven
patrol/chase AI, classic billboard 2D sprites that swap front/side/back
based on the angle to the camera. Pick up keys, open doors, shoot enemies,
reach the exit.

## Assets — all GPL / royalty-free

This game is a Doom homage. **No id Software assets ship in this repo.**

- **Sprites**: [Freedoom](https://freedoom.github.io/) (GPL-2.0) — drop-in
  replacements with Doom-compatible filenames so the same prefab references
  resolve to legally-clean sprites.
- **Music**: "Lite Saturation — Metal Energy Loop" (royalty-free metal)
- **Sound effects**: original WAV files written for this assignment
- **Engine concepts** (billboard rendering, WAD-style level layout): Doom's
  *ideas*, not its code or art. Game mechanics aren't copyrightable.

## Controls

- **W / A / S / D** — move
- **Mouse** — look
- **Left mouse** — fire
- **Space** — open door (when standing next to one)

## Scripts

| File | Role |
|---|---|
| `PlayerBehaviour.cs` | WASD movement on Rigidbody, health + armor HUD, `TakeDamage` entry point |
| `CameraController.cs` | Mouse-look (yaw on player, pitch on camera) |
| `Gun.cs` | Raycast on Fire1, calls `AIChaseCompleteBehaviour.TakeDamage` on hit |
| `Projectile.cs` | Forward-translating projectile, auto-destroys after lifetime, damages player on trigger |
| `AIChaseCompleteBehaviour.cs` | NavMeshAgent + Patrol/Chase/Stationary state machine, LOS via raycast, fires when in range |
| `BillboardSpriteBehaviour.cs` | Classic Doom-style sprite that faces the camera with front/side/back variant swap |
| `DoorController.cs` | Slide-open on Space when player is nearby, auto-close after delay |

## What was kept from the original CS 3350 assignment

- **Billboard sprite trick** — the most authentic part of the Doom homage.
  Angle-based front/side/back variant swap. Real engineering, not a copy.
- **NavMesh-driven AI** — patrol/chase state machine using Unity's
  industry-standard navigation system.
- **ProBuilder level construction** (E1M1.unity) — in-editor geometry
  modeling rather than externally-authored 3D assets.
- **Doom-style asset naming convention** (TROOA1, SPOSA1, etc.) preserved
  for compatibility with the Freedoom replacement sprites.

## What the portfolio-polish pass fixed

- **`Projectile._player` was declared but never assigned.** Original
  null-ref'd on every player hit. Now reads `PlayerBehaviour.Instance`
  directly at hit time.
- **`Projectile.Instance` static singleton was nonsense** on a multi-instance
  class — every projectile spawn overwrote it. Removed.
- **`PlayerBehaviour` armor bar showed health** (copy-paste bug from
  health). Original: `armorbar.value = health; armorText.text = health`.
  Corrected to use the armor field.
- **`PlayerBehaviour` HUD ran in `FixedUpdate`** — wrote the UI text every
  physics tick. Moved to a `RefreshHud()` call from `TakeDamage` (and
  initial `Start`), so the string builds only when values change.
- **`Gun.Shoot` damaged enemies via direct `health -=` field access**,
  bypassing any future logic. Replaced with `ai.TakeDamage(damagePerShot)`.
- **`AIChaseCompleteBehaviour.CanSeePlayer` used un-normalized raycast**
  direction (magnitude = distance, scaling arbitrarily). Now normalizes
  and passes distance as the max-distance argument.
- **`Rigidbody.velocity` deprecated in Unity 6.3.** Migrated to
  `Rigidbody.linearVelocity`, preserving Y so gravity still applies.
- **`DoorController.OpenDoor` coroutine was pointless** (yielded null
  immediately, set a bool). Replaced with direct assignment. Cached
  AudioSource. Squared-distance for `IsPlayerNearby`.
- **`BillboardSprite` class name didn't match its file** (`BillboardSpriteBehaviour.cs`).
  Unity logs an error when MonoBehaviour class+file disagree. Renamed
  class to match.
- **`namespace Games.DoomClone`** wrapped all 7 scripts to avoid
  `CameraController` / `Gun` / `Projectile` collisions with the FPS game's
  same-named classes.
- **`Debug.LogWarning` dev logging** removed (was sprinkled through Gun,
  Projectile, DoorController during original development).

## What's intentionally left as-is

- Single scene (E1M1). The exit room exists but doesn't transition — a
  proper `ExitTrigger` + multi-level setup is deferred.
- ProBuilder-built level geometry rather than externally-modeled. Demonstrates
  in-engine tooling, fits the Doom-era aesthetic.
- Minimal HUD (two thin bars + numbers). Real Doom STBAR is deferred.

## Deferred portfolio polish

Tracked in [`project-portfolio-followups`](https://github.com/TheWerbinator)
memory. Game is shippable as-is; below land later if I revisit:

- **Pickups** — green/blue armor + health packs. `PlayerBehaviour.TakeDamage`
  already routes through an `armorAbsorption` field (1/3 = green, 1/2 = blue);
  pickup just sets that field on collect.
- **Full Doom HUD** — STBAR + STKEYS + face portrait (STFB/STFST/STFEVL...)
  + ammo per weapon type. Freedoom sprites for all of this already sit in
  `Materials/Sprites/`. Current HUD is a temporary two-bar stand-in.
- **Level progression** — `ExitTrigger` MonoBehaviour that calls
  `SceneManager.LoadScene("E1M2")`. Currently the exit room is a dead end
  because E1M2 doesn't exist yet.

## Project-level setup needed

DOOM Clone requires two packages that the umbrella project picks up
automatically once you open Unity 6.3:

- `com.unity.ai.navigation` (for `NavMeshAgent`)
- `com.unity.probuilder` (to load the E1M1 ProBuilder geometry)

Both are added to `Packages/manifest.json` in this commit. Unity's
package resolver will install on first open.

## Open in Unity

`Assets/_Games/DoomClone/E1M1.unity`. First open triggers a package
install (AI Navigation + ProBuilder) and an URP material upgrade pass.
The Freedoom sprites should already render correctly — same filenames
mean the existing prefab GUIDs still resolve.
