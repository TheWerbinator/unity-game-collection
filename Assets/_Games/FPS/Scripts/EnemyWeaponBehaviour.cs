using UnityEngine;

namespace Games.FPS {
    public class EnemyWeaponBehaviour : WeaponBehaviour {
        [SerializeField] private float _cooldown = 1.5f;
        [SerializeField] private float _maxRange = 30f;
        private float _timer;

        void Update() {
            _timer += Time.deltaTime;

            if (PlayerController.Instance == null) return;

            Vector3 toPlayer = PlayerController.Instance.transform.position - transform.position;
            float distance = toPlayer.magnitude;
            if (distance > _maxRange) return;

            // Normalize the direction; the original passed the raw vector
            // whose magnitude could be huge, which can produce a ray that
            // overshoots its target's collider.
            Vector3 dirToPlayer = toPlayer / distance;

            // Single raycast that we both check for LOS and fire along.
            // Original did two raycasts back-to-back — wasted call.
            if (!Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, _maxRange)) return;
            if (hit.collider == null || !hit.collider.CompareTag("Player")) return;

            transform.LookAt(PlayerController.Instance.transform);
            if (_timer >= _cooldown) {
                _timer = 0f;
                FireWeapon(hit);
            }
        }
    }
}
