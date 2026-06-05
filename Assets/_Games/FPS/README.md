# FPS

A first-person shooter. Walk a small box level, pick up key cards to open
locked doors, dodge a patrolling enemy that shoots when it has line of
sight, reach the end point.

## Controls

- **W / A / S / D** — move
- **Mouse** — look (cursor is locked on play; released on death/win)
- **Left mouse** — fire
- **E** — open the door in front of you (requires the matching key card)

## Architecture — the OOP that earned this game most of its grade

```
CharacterBehaviour              ← shared health + Hit / Die hooks
   ├── PlayerController         ← health UI, key inventory, death disables input
   └── EnemyBehaviour           ← death ragdoll-tilt, disable enemy systems

WeaponBehaviour                 ← shared fire-projectile + damage application
   ├── PlayerWeaponBehaviour    ← raycast on mouse click, fire
   └── EnemyWeaponBehaviour     ← LOS-gated, cooldown-rated auto-fire

PlayerMovementBehaviour         ← WASD → Rigidbody linearVelocity (Unity 6 API)
EnemyMovementBehaviour          ← square-patrol AI, turn 90° on collision
CameraController                ← mouse-look, owns player yaw + camera pitch
DoorBehaviour + KeyBehaviour    ← E-to-unlock when matching key in inventory
```

11 scripts. The inheritance + composition split was the assignment's main
pedagogical goal — separate "what a character is" from "how it moves" from
"how it shoots" so future enemies/weapons can mix-and-match implementations.

## Scripts

| File | Role |
|---|---|
| `CharacterBehaviour.cs` | Base class — health, Hit, virtual Die, OnHit hook |
| `WeaponBehaviour.cs` | Base class — fire projectile, apply damage, auto-destroy projectile after lifetime |
| `PlayerController.cs` | Singleton, health UI, key inventory, death disables movement+weapon+camera |
| `PlayerMovementBehaviour.cs` | WASD → Rigidbody, win on EndPoint contact |
| `PlayerWeaponBehaviour.cs` | Raycast on left-click, fire if hit |
| `EnemyBehaviour.cs` | Death = tilt + disable enemy components |
| `EnemyMovementBehaviour.cs` | Square patrol, turn on collision |
| `EnemyWeaponBehaviour.cs` | LOS-gated fire on cooldown |
| `CameraController.cs` | Mouse-look (yaw on player, pitch on camera) |
| `DoorBehaviour.cs` | E-to-unlock when player in range + has matching key |
| `KeyBehaviour.cs` | Trigger pickup → add to PlayerController inventory |

## What the portfolio-polish pass fixed

- **`PlayerWeaponBehaviour` could null-ref on miss.** Original passed the
  uninitialized `RaycastHit` to `FireWeapon` regardless of whether the
  raycast actually hit anything; a missed shot would throw on the
  `hit.transform` read inside the base class. Now gated by the bool return
  of `Physics.Raycast`.
- **`EnemyWeaponBehaviour` had un-normalized raycast + redundant second
  raycast.** Direction vector `toPlayer` was passed raw — its magnitude
  could overshoot the player's collider. And after LOS confirmation, the
  code did a second redundant raycast along the same direction. Collapsed
  to one normalized raycast, added a configurable `_maxRange`, gated on
  player-tag.
- **`WeaponBehaviour` projectile-spawn was a memory leak.** Each shot
  parented the projectile to its target with no destruction path —
  wall-hits accumulated forever. Added `Destroy(projectile, _projectileLifetime)`
  on spawn (default 10s).
- **`Rigidbody.velocity` deprecated in Unity 6.3.** Migrated to
  `Rigidbody.linearVelocity` in PlayerMovementBehaviour and
  EnemyMovementBehaviour. Y-velocity preserved so gravity still applies.
- **Health UI rewrote every frame.** Original updated `_healthUIText.text`
  in `Update`. Moved to an `OnHit` override (and `Start`) so the string
  builds only when the value changes.
- **`PlayerMovementBehaviour.OnCollisionEnter` for EndPoint was a stub.**
  Original only `Debug.Log`'d — no visible win state. Now activates a
  `_winWindow` UI and disables further input.
- **Death-state cleanup added.** `PlayerController.Die` now also
  releases the cursor lock so the player can click the failure window.
- **Cached AudioSource** in `WeaponBehaviour.Awake` instead of
  per-fire `GetComponent`.
- **`namespace Games.FPS`** wrapped all 11 scripts to avoid the
  collision with Platformer's `PlayerController` and `EnemyController` (same names).

## What's intentionally left as-is

- Square-patrol AI (`int _direction = 1..4`). Could be NavMesh or
  waypoint-based, but the level is a small box and the simple version
  reads cleaner than overkill pathfinding.
- Single scene. No multi-level progression. Original scope.
- Camera owns player yaw. Unusual coupling but the standard FPS idiom.

## Open in Unity

`Assets/_Games/FPS/Scenes/Main.unity`.

## Unity setup needed

- **Player tag** on the player root GameObject
- **Enemy tag** on enemy prefab
- **EndPoint tag** on whatever object/trigger represents the goal
- **PlayerController _failureWindow** + **PlayerMovementBehaviour _winWindow**: drag the corresponding UI panels in the Inspector (or leave null — they're optional now thanks to null-guards)
- If you reload the scene mid-development, check the cursor lock — `Cursor.lockState = CursorLockMode.Locked` happens in Start
