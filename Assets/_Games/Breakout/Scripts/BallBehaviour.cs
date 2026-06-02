using UnityEngine;

namespace Games.Breakout {
    public class BallBehaviour : MonoBehaviour {
        public Vector3 Velocity;
        public float MaxHorizontalVelocity;

        // Physics in FixedUpdate so behavior is frame-rate independent — the
        // original ran movement in Update which gave different speeds at
        // 60 Hz vs 144 Hz.
        void FixedUpdate() {
            if (!GameManager.Instance.GameRunning) return;

            if (CollidesWithGameObject(PaddleController.Instance.gameObject)) {
                // Preserve total speed across paddle bounce. The original
                // formula (Velocity.x = paddleToBall.x * MaxHorizontalVelocity)
                // increased the ball's speed every time it hit the paddle edge,
                // making the game accelerate uncontrollably. Compute the new
                // direction from the paddle-relative hit point, then re-scale
                // to the speed the ball had before the bounce.
                float speed = Velocity.magnitude;
                Vector3 paddleToBall = transform.position - PaddleController.Instance.gameObject.transform.position;
                Vector3 newDir = new Vector3(
                    paddleToBall.x * MaxHorizontalVelocity,
                    Mathf.Abs(Velocity.y),
                    0
                ).normalized;
                Velocity = newDir * speed;
            }

            for (int i = 0; i < GameManager.Instance.Bricks.Length; i++) {
                if (GameManager.Instance.Bricks[i] == null) continue;
                if (CollidesWithGameObject(GameManager.Instance.Bricks[i])) {
                    Destroy(GameManager.Instance.Bricks[i]);
                    GameManager.Instance.Bricks[i] = null;
                    Velocity.y = -Velocity.y;
                    GameManager.Instance.CheckForWin(gameObject);
                    break;
                }
            }

            // Wall bounces only when we're past the bound AND moving further
            // out — without the direction check the ball can flicker against
            // the wall if it overshoots and the next frame's reverse pushes
            // it back inside.
            float maxX = GameManager.Instance.MaxBounds.transform.position.x;
            float minX = GameManager.Instance.MinBounds.transform.position.x;
            float maxY = GameManager.Instance.MaxBounds.transform.position.y;

            if (transform.position.y > maxY && Velocity.y > 0) {
                Velocity.y = -Velocity.y;
            }
            if ((transform.position.x > maxX && Velocity.x > 0) ||
                (transform.position.x < minX && Velocity.x < 0)) {
                Velocity.x = -Velocity.x;
            }

            if (transform.position.y < GameManager.Instance.MinBounds.transform.position.y) {
                GameManager.Instance.BallOutOfBounds(gameObject);
                return;
            }

            transform.position += Velocity * Time.fixedDeltaTime;
        }

        // Axis-aligned bounding-box overlap test. The original implementation
        // had an operator-precedence bug that made the function return true
        // in cases where the boxes did not actually overlap; corrected here
        // to the standard AND-per-axis form.
        private bool CollidesWithGameObject(GameObject other) {
            Vector3 aPos = transform.position;
            Vector3 aHalf = transform.localScale * 0.5f;
            Vector3 bPos = other.transform.position;
            Vector3 bHalf = other.transform.localScale * 0.5f;
            return (aPos.x - aHalf.x) <= (bPos.x + bHalf.x) &&
                   (aPos.x + aHalf.x) >= (bPos.x - bHalf.x) &&
                   (aPos.y - aHalf.y) <= (bPos.y + bHalf.y) &&
                   (aPos.y + aHalf.y) >= (bPos.y - bHalf.y);
        }
    }
}
