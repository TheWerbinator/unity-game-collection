using UnityEngine;

namespace Games.FPS {
    public class DoorBehaviour : MonoBehaviour {
        [SerializeField] private KeyBehaviour _keyForDoor;
        private bool _playerInRange;

        void Update() {
            if (!_playerInRange) return;
            if (!Input.GetKeyDown(KeyCode.E)) return;
            if (PlayerController.Instance == null) return;
            if (!PlayerController.Instance.KeysCollected.Contains(_keyForDoor)) return;
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) _playerInRange = true;
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player")) _playerInRange = false;
        }
    }
}
