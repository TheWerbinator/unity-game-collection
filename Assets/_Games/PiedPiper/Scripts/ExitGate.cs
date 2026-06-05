using UnityEngine;

namespace Games.PiedPiper {
    /// <summary>
    /// The exit gate. Stays dim and non-functional until the Piper has
    /// collected the quota of children, then lights up and accepts the
    /// Piper as a win trigger.
    /// </summary>
    /// <remarks>
    /// Visuals are driven through a MaterialPropertyBlock so this works
    /// across URP / HDRP / built-in pipelines without instantiating the
    /// shared material. Also scales up on activate so the change is
    /// visible at a glance.
    /// </remarks>
    public class ExitGate : MonoBehaviour {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Color _inactiveColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color _activeColor = new Color(0.2f, 1f, 0.4f);
        [SerializeField] private float _activeScaleBump = 1.25f;

        private bool _active;
        private MaterialPropertyBlock _block;
        private Vector3 _baseScale;

        void Awake() {
            if (_renderer == null) {
                _renderer = GetComponent<MeshRenderer>();
            }
            _block = new MaterialPropertyBlock();
            _baseScale = transform.localScale;
        }

        void Start() {
            SetActiveVisual(false);
        }

        public void Activate() {
            if (_active) return;
            _active = true;
            SetActiveVisual(true);
            transform.localScale = _baseScale * _activeScaleBump;
            Debug.Log("ExitGate activated.");
        }

        private void SetActiveVisual(bool on) {
            if (_renderer == null) {
                Debug.LogWarning("ExitGate has no MeshRenderer wired.", this);
                return;
            }
            Color c = on ? _activeColor : _inactiveColor;
            _renderer.GetPropertyBlock(_block);
            // URP Lit reads _BaseColor; legacy reads _Color. Setting both
            // covers any shader pipeline.
            _block.SetColor("_BaseColor", c);
            _block.SetColor("_Color", c);
            _block.SetColor("_EmissionColor", on ? _activeColor * 1.5f : Color.black);
            _renderer.SetPropertyBlock(_block);
        }

        void OnTriggerEnter(Collider other) {
            if (!_active) return;
            if (other.CompareTag("Player")) {
                GameController.Instance.Win = true;
            }
        }
    }
}
