using UnityEngine;

namespace Games.FPS {
    public class PlayerMovementBehaviour : MonoBehaviour {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private GameObject _winWindow;

        private Rigidbody _rigidBody;

        void Start() {
            Cursor.lockState = CursorLockMode.Locked;
            _rigidBody = GetComponent<Rigidbody>();
            if (_winWindow != null) _winWindow.SetActive(false);
        }

        void FixedUpdate() {
            Vector3 input = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
            if (Input.GetKey(KeyCode.A)) input += Vector3.left;
            if (Input.GetKey(KeyCode.S)) input += Vector3.back;
            if (Input.GetKey(KeyCode.D)) input += Vector3.right;

            input = input.normalized;
            Vector3 velocity = (input.z * transform.forward + input.x * transform.right) * _movementSpeed;
            // Preserve gravity-driven vertical velocity — only override XZ.
            velocity.y = _rigidBody.linearVelocity.y;
            _rigidBody.linearVelocity = velocity;
        }

        private void OnTriggerEnter(Collider other) {
            // EndPoint is a trigger collider — player passes into the goal
            // pad rather than bonking off a wall. Original only Debug.Log'd
            // here; now activates the win UI and locks down further input.
            if (!other.CompareTag("EndPoint")) return;
            if (_winWindow != null) _winWindow.SetActive(true);
            enabled = false;
            PlayerWeaponBehaviour weapon = GetComponent<PlayerWeaponBehaviour>();
            if (weapon != null) weapon.enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
