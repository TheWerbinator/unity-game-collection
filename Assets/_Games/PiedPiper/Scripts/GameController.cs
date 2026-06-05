using System.Collections;
using TMPro;
using UnityEngine;

namespace Games.PiedPiper {
    public class GameController : MonoBehaviour {
        public static GameController Instance;

        public bool NeedNewChild = false;
        public float MovementSpeed;
        public bool Lose;
        public bool Win;

        [Header("Quota / Exit")]
        [SerializeField] private int _quota = 5;
        [SerializeField] private ExitGate _exitGate;
        [SerializeField] private TextMeshProUGUI _quotaText;

        [Header("Spawn / Loss")]
        [SerializeField] private GameObject _childPrefab;
        [SerializeField] private GameObject _lossScreen;
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private float _childSpawnInterval = 1.5f;

        private float _spawnTimer;
        private int _childrenCollected;
        private bool _lossFlag;
        private bool _winFlag;

        public int Quota => _quota;
        public int ChildrenCollected => _childrenCollected;

        void Start() {
            Instance = this;
            if (_lossScreen != null) _lossScreen.SetActive(false);
            if (_winScreen != null) _winScreen.SetActive(false);
            UpdateQuotaText();
        }

        void Update() {
            // Stop spawning once quota is met. The Piper has all the children
            // they need; let them focus on finding the exit.
            if (_childrenCollected >= _quota) {
                NeedNewChild = false;
            }

            if (NeedNewChild) {
                _spawnTimer -= Time.deltaTime;
                if (_spawnTimer <= 0f) {
                    CreateChild();
                    NeedNewChild = false;
                    _spawnTimer = _childSpawnInterval;
                }
            }

            if (Lose && !_lossFlag) {
                _lossFlag = true;
                StartCoroutine(ProcessLoss());
            }
            if (Win && !_winFlag) {
                _winFlag = true;
                ProcessWin();
            }
        }

        public void CreateChild() {
            MovementSpeed *= 1.01f;
            Vector3 initPosition = new Vector3(
                Random.Range(-19f, 19f),
                0f,
                Random.Range(-9f, 9f)
            );
            GameObject child = Instantiate(_childPrefab, initPosition, Quaternion.identity);
            Collider col = child.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }

        /// <summary>
        /// Called by PiperBehaviour each time a child is recruited. Bumps
        /// the count, refreshes UI, and activates the exit when the quota
        /// is reached.
        /// </summary>
        public void RecordChildPickup() {
            _childrenCollected++;
            UpdateQuotaText();
            if (_childrenCollected >= _quota && _exitGate != null) {
                _exitGate.Activate();
            }
        }

        private void UpdateQuotaText() {
            if (_quotaText == null) return;
            int remaining = Mathf.Max(0, _quota - _childrenCollected);
            _quotaText.text = remaining > 0
                ? $"Children: {_childrenCollected} / {_quota}"
                : "Children: full — head for the exit!";
        }

        private IEnumerator ProcessLoss() {
            MovementSpeed = 0f;
            if (_lossScreen != null) _lossScreen.SetActive(true);
            AudioSource selfAudio = GetComponent<AudioSource>();
            if (selfAudio != null) selfAudio.Stop();
            if (_lossScreen != null) {
                AudioSource lossAudio = _lossScreen.GetComponent<AudioSource>();
                if (lossAudio != null) lossAudio.Play();
            }

            // Scatter everyone — followers in the chain and any idle children
            // still wandering. Each picks a random direction and runs off the
            // map. Self-destructs when they cross the boundary.
            foreach (ChildBehaviour c in Object.FindObjectsByType<ChildBehaviour>(FindObjectsSortMode.None)) {
                if (c != null) c.StartScatter();
            }
            yield break;
        }

        private void ProcessWin() {
            MovementSpeed = 0f;
            if (_winScreen != null) _winScreen.SetActive(true);
            AudioSource selfAudio = GetComponent<AudioSource>();
            if (selfAudio != null) selfAudio.Stop();
        }
    }
}
