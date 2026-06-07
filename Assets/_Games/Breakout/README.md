# Breakout

The classic paddle-and-bricks game. Hand-rolled AABB collision (no
`Rigidbody2D`), paddle-relative bounce angle, and a brick array
populated by hand in the inspector.

## Controls

- **← / →** — move paddle
- Brick destroyed on each hit; clear them all to win
- 5 lives by default (configurable in `GameManager._maxRespawns`)

## Scripts

| File | Role |
|---|---|
| `BallBehaviour.cs` | Per-frame physics step + collision against paddle/bricks/walls |
| `GameManager.cs` | Win/lose state, ball respawn, UI screens, brick inventory |
| `PaddleController.cs` | Keyboard input → bounded horizontal movement |

## What was kept from the original CS 3350 assignment

- Manual AABB collision instead of `Rigidbody2D` + `OnCollisionEnter2D` — the
  assignment's lesson was to implement collision detection without leaning
  on Unity's physics engine. Kept intact (with the bug fix below).
- Paddle-relative bounce angle: where the ball hits the paddle determines
  its horizontal velocity. Classic Breakout feel; very few tutorials get
  this right.
- Brick array populated by hand in the inspector (the original assignment
  didn't ask for programmatic level generation).

## What the portfolio-polish pass fixed

- **AABB had an operator-precedence bug.** The `||` / `&&` chain in
  `CollidesWithGameObject` made the function return true in cases where
  the boxes didn't actually overlap. Worked "well enough" in practice
  because the ball moves fast and other corrections kicked in, but it was
  wrong. Replaced with the standard AND-per-axis form.
- **Physics moved from `Update` to `FixedUpdate`.** The original ran
  movement in `Update`, giving different ball speeds at 60 Hz vs 144 Hz.
  `FixedUpdate` is frame-rate independent.
- **Wall-bounce now checks velocity direction.** Without that check, a
  ball that overshoots the wall by enough to still be out-of-bounds next
  frame can flicker as the bounce keeps flipping its velocity.
- **Removed dead code**: empty `GameManager.Update()` and a Space-bar
  paddle-color toggle that did nothing functional.
- **Ball preserves speed across paddle bounces.** The original directly
  assigned `Velocity.x = paddleToBall.x * MaxHorizontalVelocity`, which
  combined with the unchanged `Velocity.y` gave the ball a new total
  speed of `sqrt(new_x² + old_y²)` after every hit — the game
  accelerated uncontrollably. The polished version computes the new
  direction the same way, then renormalizes to the pre-bounce speed.
- **Replaced missing sprite asset.** The original referenced a sprite
  GUID that didn't survive the hand-off; the polish ships a
  procedurally-generated 32×32 white square at 32 PPU = 1 world unit per
  scale-1, so the existing transforms render at their intended sizes.
- **Project migrated 2022.3 LTS → Unity 6.3 LTS** with URP 14 → 17.
  Scripts unchanged across the upgrade; URP materials regenerated.

## Open in Unity

This is one game inside the `unity-game-collection` Unity project. From the
project root, open Unity Hub → Add project → select `unity-game-collection`,
then open `Assets/_Games/Breakout/Scenes/Breakout.unity`.
