using UnityEngine;
using UnityEngine.AI;

namespace Games.DoomClone {
    public class AIChaseCompleteBehaviour : MonoBehaviour {
        public enum AIState { Patrol, Chase, Stationary }

        public float shootingRange = 10f;
        public float fireRate = 1f;
        public Projectile projectilePrefab;
        public Transform firePoint;
        public LayerMask obstacleMask = ~0;
        public int health = 10;

        [SerializeField] private AIState _state;
        [SerializeField] private float _outOfSightChaseTime = 4f;

        private float _chaseTimer;
        private NavMeshAgent _agent;
        private float _nextFireTime;

        void Start() {
            _agent = GetComponent<NavMeshAgent>();
            _state = AIState.Patrol;
            _chaseTimer = _outOfSightChaseTime;
        }

        void Update() {
            if (health <= 0) {
                Die();
                return;
            }
            // Look up the player via the singleton rather than a serialized
            // field. Eliminates per-prefab Inspector wiring and dodges the
            // "wrong type" error from dragging a scene GameObject into a
            // field whose serializer expected an asset.
            PlayerBehaviour playerRef = PlayerBehaviour.Instance;
            if (playerRef == null) return;
            GameObject playerGo = playerRef.gameObject;

            _chaseTimer += Time.deltaTime;
            float distanceToPlayer = Vector3.Distance(transform.position, playerGo.transform.position);

            if (CanSeePlayer(playerGo)) {
                _chaseTimer = 0f;
                if (distanceToPlayer <= shootingRange && Time.time >= _nextFireTime) {
                    Shoot(playerGo);
                    _nextFireTime = Time.time + 1f / fireRate;
                }
            }

            if (_state == AIState.Stationary) return;

            if (_chaseTimer < _outOfSightChaseTime) {
                Chase(playerGo);
            } else {
                _state = AIState.Patrol;
            }

            if (_agent.remainingDistance > _agent.stoppingDistance) return;
            if (_state == AIState.Patrol) Patrol();
        }

        public void TakeDamage(int amount) {
            health -= amount;
        }

        private bool CanSeePlayer(GameObject player) {
            // Normalize direction so the raycast magnitude matches what the
            // caller asked for. Original passed the un-normalized vector,
            // which scales arbitrarily with distance.
            Vector3 toPlayer = player.transform.position - transform.position;
            float dist = toPlayer.magnitude;
            if (dist < 0.001f) return true;
            if (!Physics.Raycast(transform.position, toPlayer / dist, out RaycastHit hit, dist, obstacleMask)) {
                return false;
            }
            return hit.collider != null && hit.collider.gameObject == player;
        }

        private void Patrol() {
            // Random NavMesh-sampled wander. 100 retries is generous; in
            // practice the first sample usually lands somewhere valid.
            for (int i = 0; i < 100; i++) {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-10f, 10f),
                    0f,
                    Random.Range(-10f, 10f)
                );
                if (NavMesh.SamplePosition(transform.position + randomDirection, out NavMeshHit navmeshHit, 100f, 1)) {
                    _agent.destination = navmeshHit.position;
                    return;
                }
            }
        }

        private void Chase(GameObject player) {
            _agent.destination = player.transform.position;
            _state = AIState.Chase;
        }

        private void Shoot(GameObject player) {
            if (projectilePrefab == null || firePoint == null) return;
            // Compute rotation toward the player rather than using
            // firePoint.rotation. The billboard sprite rotates the parent
            // transform to face the camera, so firePoint inherits that
            // facing — using its rotation directly fires projectiles away
            // from the player instead of toward them.
            Vector3 dirToPlayer = (player.transform.position - firePoint.position).normalized;
            Quaternion rot = dirToPlayer.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(dirToPlayer)
                : Quaternion.identity;
            Instantiate(projectilePrefab, firePoint.position, rot);
        }

        private void Die() {
            Destroy(gameObject);
        }
    }
}
