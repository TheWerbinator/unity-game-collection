using UnityEngine;

namespace Games.FPS {
    /// <summary>
    /// Shared health + death behaviour for Player and Enemy. Subclasses
    /// override <see cref="Die"/> to specify what happens on death — show
    /// failure UI, ragdoll, disable input, etc.
    /// </summary>
    public class CharacterBehaviour : MonoBehaviour {
        [SerializeField, Range(0, 10)] protected int _maxHealth;
        protected int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        protected virtual void Start() {
            _currentHealth = _maxHealth;
        }

        public void Hit() {
            _currentHealth--;
            OnHit();
            if (_currentHealth <= 0) Die();
        }

        // Hook for subclasses to react to taking damage without overriding
        // Hit itself (e.g. refresh HUD without re-implementing the decrement).
        protected virtual void OnHit() { }

        public virtual void Die() { }
    }
}
