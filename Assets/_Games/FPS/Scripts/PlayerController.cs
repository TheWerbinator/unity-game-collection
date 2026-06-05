using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.FPS {
    public class PlayerController : CharacterBehaviour {
        public static PlayerController Instance;
        public List<KeyBehaviour> KeysCollected = new List<KeyBehaviour>();

        [SerializeField] private GameObject _failureWindow;
        [SerializeField] private TextMeshProUGUI _healthUIText;

        void Awake() {
            Instance = this;
            if (_failureWindow != null) _failureWindow.SetActive(false);
        }

        protected override void Start() {
            base.Start();
            RefreshHealthUI();
        }

        protected override void OnHit() {
            RefreshHealthUI();
        }

        public override void Die() {
            if (_failureWindow != null) _failureWindow.SetActive(true);
            // Disable input/control on death; avoids the corpse continuing
            // to slide around or shooting from beyond the grave.
            PlayerMovementBehaviour move = GetComponent<PlayerMovementBehaviour>();
            if (move != null) move.enabled = false;
            PlayerWeaponBehaviour weapon = GetComponent<PlayerWeaponBehaviour>();
            if (weapon != null) weapon.enabled = false;
            CameraController cam = GetComponentInChildren<CameraController>();
            if (cam != null) cam.enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }

        private void RefreshHealthUI() {
            if (_healthUIText != null) {
                _healthUIText.text = $"Health: {_currentHealth}/{_maxHealth}";
            }
        }
    }
}
