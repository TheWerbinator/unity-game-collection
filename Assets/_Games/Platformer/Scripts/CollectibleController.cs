using UnityEngine;

namespace Games.Platformer {
    public class CollectibleController : MonoBehaviour {
        [SerializeField] private AudioClip _collectSound;

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;
            if (_collectSound != null) {
                AudioSource.PlayClipAtPoint(_collectSound, transform.position);
            }
            GameController.Instance.UpdateScore();
            Destroy(gameObject);
        }
    }
}
