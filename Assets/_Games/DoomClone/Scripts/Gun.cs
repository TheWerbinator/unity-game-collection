using UnityEngine;

namespace Games.DoomClone {
    /// <summary>
    /// Doom-style hitscan with vertical autoaim. The player faces left/right
    /// only (no mouse-look pitch in the original engine); shooting fires a
    /// "tall" hitscan that auto-snaps vertically to any enemy inside a
    /// horizontal cone in front of the player. No need to aim up at an imp
    /// on a ledge — if it's in front of you and within range, you can hit it.
    /// </summary>
    public class Gun : MonoBehaviour {
        [Tooltip("Camera used only to read the player's facing direction (yaw).")]
        public Camera playerCamera;
        public float shootingRange = 100f;
        [Tooltip("Half-angle of the horizontal autoaim cone, in degrees. " +
                 "Doom's cone is roughly 5°; widen for forgiveness.")]
        public float autoaimConeAngle = 10f;
        public LayerMask shootableLayer = ~0;
        [Tooltip("Layers a wall/floor sits on. Used for line-of-sight checks. " +
                 "Defaults to Everything; narrow if you have walls + enemies on " +
                 "different layers and don't want enemies to block each other.")]
        public LayerMask losBlockingLayer = ~0;
        public int damagePerShot = 10;
        public float fireRate = 0.2f;

        private float _nextFireTime;
        private AudioSource _audio;

        void Awake() {
            _audio = GetComponent<AudioSource>();
        }

        void Update() {
            if (!Input.GetButton("Fire1")) return;
            if (Time.time < _nextFireTime) return;

            if (_audio != null) _audio.Play();
            Shoot();
            _nextFireTime = Time.time + fireRate;
        }

        private void Shoot() {
            AIChaseCompleteBehaviour target = FindAutoaimTarget();
            if (target == null) return;
            target.TakeDamage(damagePerShot);
        }

        /// <summary>
        /// Doom autoaim. Sweep every Enemy collider in the scene, keep the
        /// ones inside the horizontal cone in front of the player and within
        /// range, line-of-sight-verify each, and return the closest.
        /// </summary>
        private AIChaseCompleteBehaviour FindAutoaimTarget() {
            Vector3 origin = playerCamera != null ? playerCamera.transform.position : transform.position;
            Vector3 forwardHorizontal = playerCamera != null
                ? Flatten(playerCamera.transform.forward)
                : Flatten(transform.forward);
            if (forwardHorizontal.sqrMagnitude < 0.0001f) return null;

            AIChaseCompleteBehaviour best = null;
            float bestDist = float.PositiveInfinity;

            // Sphere overlap to find candidates within range, then filter
            // by horizontal cone. Cheaper than scanning every GameObject.
            Collider[] hits = Physics.OverlapSphere(origin, shootingRange, shootableLayer);
            foreach (Collider c in hits) {
                if (c == null) continue;
                if (!c.CompareTag("Enemy")) continue;

                Vector3 toEnemy = c.bounds.center - origin;
                Vector3 toEnemyHorizontal = Flatten(toEnemy);
                if (toEnemyHorizontal.sqrMagnitude < 0.0001f) continue;

                // Horizontal angle only — vertical is forgiven by autoaim.
                float angle = Vector3.Angle(forwardHorizontal, toEnemyHorizontal);
                if (angle > autoaimConeAngle) continue;

                // Line-of-sight check using full 3D direction. The raycast
                // MUST hit this enemy as its first collider — if it hits
                // anything else (wall, door, decoration), the shot is
                // blocked. If the raycast misses entirely, the enemy's own
                // collider isn't on the LOS layer mask and we can't verify
                // line-of-sight — treat as blocked rather than auto-pass
                // (the original bug let bullets sail through walls).
                float distance = toEnemy.magnitude;
                if (!Physics.Raycast(origin, toEnemy.normalized, out RaycastHit losHit, distance, losBlockingLayer)) {
                    continue;
                }
                if (losHit.collider != c) continue;  // wall/door in the way

                if (distance < bestDist) {
                    AIChaseCompleteBehaviour ai = c.GetComponent<AIChaseCompleteBehaviour>();
                    if (ai != null) {
                        bestDist = distance;
                        best = ai;
                    }
                }
            }

            return best;
        }

        private static Vector3 Flatten(Vector3 v) {
            v.y = 0f;
            return v.sqrMagnitude < 0.0001f ? Vector3.zero : v.normalized;
        }
    }
}
