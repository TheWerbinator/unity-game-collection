using UnityEngine;

namespace Games.FPS {
    public class KeyBehaviour : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) return;
            if (PlayerController.Instance == null) return;
            // Use SetActive(false) rather than Destroy so the reference
            // tracked in PlayerController.KeysCollected stays valid for
            // door-unlock lookups (the door's _keyForDoor is a serialized
            // ref to this script — destroying nukes the comparison).
            PlayerController.Instance.KeysCollected.Add(this);
            gameObject.SetActive(false);
        }
    }
}
