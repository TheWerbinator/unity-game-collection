using UnityEngine;

namespace Games.Platformer {
    public class CheckpointBehaviour : MonoBehaviour {
        [SerializeField] private AudioClip _checkpointSound;
        [SerializeField] private bool _finalCheckpoint = false;

        void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;
            if (_checkpointSound != null) {
                AudioSource.PlayClipAtPoint(_checkpointSound, transform.position);
            }
            GameController.Instance.NextLevel(_finalCheckpoint);
        }
    }
}
