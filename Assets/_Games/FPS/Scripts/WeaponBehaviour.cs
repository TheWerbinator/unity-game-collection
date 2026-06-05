using UnityEngine;

namespace Games.FPS {
    public class WeaponBehaviour : MonoBehaviour {
        [SerializeField] private GameObject _projectilePrefab;
        // Projectiles parented to whatever they hit can pile up forever
        // (especially when shots land on walls). Auto-destroy after this many
        // seconds so the scene doesn't accumulate spent rounds.
        [SerializeField] private float _projectileLifetime = 10f;

        private AudioSource _audio;

        protected virtual void Awake() {
            _audio = GetComponent<AudioSource>();
        }

        public void FireWeapon(RaycastHit hit) {
            if (hit.collider == null) return;

            GameObject projectile = Instantiate(_projectilePrefab);
            if (_audio != null) _audio.Play();

            projectile.transform.position = hit.point;
            // Parent to the hit so a projectile stuck in a wall or enemy
            // moves with that target visually. Auto-destroy after a few
            // seconds to bound memory.
            projectile.transform.SetParent(hit.transform, worldPositionStays: true);
            Destroy(projectile, _projectileLifetime);

            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player")) {
                CharacterBehaviour character = hit.collider.GetComponent<CharacterBehaviour>();
                if (character != null) character.Hit();
            }
        }
    }
}
