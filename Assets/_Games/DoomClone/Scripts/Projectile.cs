using UnityEngine;

namespace Games.DoomClone {
    public class Projectile : MonoBehaviour {
        public float speed = 20f;
        public float lifetime = 2f;
        public int damage = 10;
        [Tooltip("One-shot sound played at impact point. Optional.")]
        public AudioClip impactSound;
        [Tooltip("Volume scale for the impact sound.")]
        [Range(0f, 1f)] public float impactVolume = 1f;

        private AudioSource _audio;

        void Start() {
            _audio = GetComponent<AudioSource>();
            if (_audio != null) _audio.Play();
            // Bounded lifetime so missed projectiles don't accumulate.
            Destroy(gameObject, lifetime);
        }

        void Update() {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other) {
            // Ignore other enemies (don't let a fireball detonate as it
            // spawns inside its own shooter) and ignore other projectiles.
            if (other.CompareTag("Enemy")) return;
            if (other.GetComponent<Projectile>() != null) return;

            if (other.CompareTag("Player") && PlayerBehaviour.Instance != null) {
                PlayerBehaviour.Instance.TakeDamage(damage);
            }

            // Play impact sound at the hit point (not on the destroyed
            // projectile — that would cut the audio short). PlayClipAtPoint
            // spawns a one-shot AudioSource that survives our Destroy call.
            if (impactSound != null) {
                AudioSource.PlayClipAtPoint(impactSound, transform.position, impactVolume);
            }

            Destroy(gameObject);
        }
    }
}
