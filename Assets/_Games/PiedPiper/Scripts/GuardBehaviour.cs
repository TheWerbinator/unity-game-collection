using UnityEngine;

namespace Games.PiedPiper {
    /// <summary>
    /// A city guard that patrols between waypoints and kills the Piper on
    /// contact. Movement is plain MoveTowards (no NavMesh) — the play area
    /// is flat and obstacle-free, so the simpler approach reads cleaner
    /// than baking a navigation surface.
    /// </summary>
    public class GuardBehaviour : MonoBehaviour {
        /// <summary>Waypoints the guard cycles through. Needs at least 2.</summary>
        [SerializeField] private Transform[] _waypoints;
        [SerializeField] private float _speed = 4f;
        [SerializeField] private float _arriveThreshold = 0.1f;

        private int _targetIndex;
        private bool _patrolForward = true;

        void Start() {
            if (_waypoints == null || _waypoints.Length < 2) {
                Debug.LogWarning($"{name}: GuardBehaviour needs at least 2 waypoints to patrol.", this);
                enabled = false;
                return;
            }
            // Snap to the first waypoint so we don't drift in from wherever
            // we were placed in the editor.
            transform.position = _waypoints[0].position;
        }

        void Update() {
            if (GameController.Instance != null && (GameController.Instance.Lose || GameController.Instance.Win)) {
                return;
            }

            Transform target = _waypoints[_targetIndex];
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                _speed * Time.deltaTime
            );

            // Face the direction of travel — keeps the rook visually oriented.
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.001f) {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(toTarget),
                    10f * Time.deltaTime
                );
            }

            if (Vector3.Distance(transform.position, target.position) < _arriveThreshold) {
                AdvanceWaypoint();
            }
        }

        private void AdvanceWaypoint() {
            // Ping-pong patrol rather than wrap-around. Feels more like a
            // beat-cop walking a route.
            if (_patrolForward) {
                _targetIndex++;
                if (_targetIndex >= _waypoints.Length) {
                    _targetIndex = _waypoints.Length - 2;
                    _patrolForward = false;
                }
            } else {
                _targetIndex--;
                if (_targetIndex < 0) {
                    _targetIndex = 1;
                    _patrolForward = true;
                }
            }
        }

        void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                GameController.Instance.Lose = true;
            }
        }

        // Editor-only: draw the patrol path so you can see it in the Scene view.
        void OnDrawGizmosSelected() {
            if (_waypoints == null) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < _waypoints.Length; i++) {
                if (_waypoints[i] == null) continue;
                Gizmos.DrawWireSphere(_waypoints[i].position, 0.3f);
                if (i + 1 < _waypoints.Length && _waypoints[i + 1] != null) {
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
                }
            }
        }
    }
}
