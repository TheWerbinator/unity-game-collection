using UnityEngine;

namespace Games.FPS {
    /// <summary>
    /// Simple square-patrol AI: walks in one cardinal direction until it
    /// hits something, then turns 90° and picks the next direction. Not
    /// pathfinding — intentional. The level is a small box; this reads as
    /// "guard walking a beat".
    /// </summary>
    public class EnemyMovementBehaviour : MonoBehaviour {
        [SerializeField] private float _movementSpeed;
        private Rigidbody _rigidBody;
        private int _direction = 1;

        void Start() {
            _rigidBody = GetComponent<Rigidbody>();
        }

        void FixedUpdate() {
            // Movement in FixedUpdate for frame-rate-independent physics;
            // original ran this in Update which gave the enemy speed
            // proportional to display refresh rate.
            Vector3 movementDirection = _direction switch {
                1 => Vector3.forward,
                2 => Vector3.right,
                3 => Vector3.back,
                4 => Vector3.left,
                _ => Vector3.forward,
            };
            Vector3 v = movementDirection * _movementSpeed;
            v.y = _rigidBody.linearVelocity.y;  // keep gravity
            _rigidBody.linearVelocity = v;
        }

        private void OnCollisionEnter(Collision collision) {
            // Rotate the model so its facing tracks where it's now walking.
            transform.Rotate(Vector3.up, 90f);
            _direction = _direction == 4 ? 1 : _direction + 1;
        }
    }
}
