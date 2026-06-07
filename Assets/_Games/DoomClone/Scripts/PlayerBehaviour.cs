using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.DoomClone {
    public class PlayerBehaviour : MonoBehaviour {
        public static PlayerBehaviour Instance;

        public int health = 100;
        public int armor = 100;

        public Slider healthBar;
        public TextMeshProUGUI healthText;
        public Slider armorbar;
        public TextMeshProUGUI armorText;

        [SerializeField] private float _movementSpeed;
        private Rigidbody _rigidBody;

        void Awake() {
            Instance = this;
        }

        void Start() {
            _rigidBody = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            RefreshHud();
        }

        void FixedUpdate() {
            Vector3 input = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
            if (Input.GetKey(KeyCode.A)) input += Vector3.left;
            if (Input.GetKey(KeyCode.S)) input += Vector3.back;
            if (Input.GetKey(KeyCode.D)) input += Vector3.right;
            input = input.normalized;

            Vector3 velocity = (input.z * transform.forward + input.x * transform.right) * _movementSpeed;
            velocity.y = _rigidBody.linearVelocity.y;  // preserve gravity
            _rigidBody.linearVelocity = velocity;
        }

        [Header("Armor absorption")]
        [Tooltip("Fraction of incoming damage absorbed by armor while armor > 0. " +
                 "Doom's green armor was 1/3 (0.33), blue armor was 1/2 (0.5).")]
        [Range(0f, 1f)] public float armorAbsorption = 1f / 3f;

        /// <summary>
        /// Called by Projectile.OnTriggerEnter when an enemy shot lands.
        /// Routes damage through Doom-style armor absorption: while armor
        /// remains, a fraction of incoming damage is soaked by armor and
        /// the rest passes through to health. When armor hits zero, all
        /// further damage goes straight to health.
        /// </summary>
        public void TakeDamage(int amount) {
            if (amount <= 0) return;

            if (armor > 0) {
                int armorAbsorb = Mathf.Min(armor, Mathf.CeilToInt(amount * armorAbsorption));
                armor -= armorAbsorb;
                health -= (amount - armorAbsorb);
            } else {
                health -= amount;
            }

            if (health < 0) health = 0;
            if (armor < 0) armor = 0;
            RefreshHud();
        }

        private void RefreshHud() {
            if (healthBar != null) healthBar.value = health;
            if (healthText != null) healthText.text = health.ToString();
            // Armor bar + text were copy-pasted from health in the original
            // (showed health, not armor). Corrected here.
            if (armorbar != null) armorbar.value = armor;
            if (armorText != null) armorText.text = armor.ToString();
        }
    }
}
