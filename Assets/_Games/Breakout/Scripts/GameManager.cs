using System.Collections;
using TMPro;
using UnityEngine;

namespace Games.Breakout {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        public Transform MinBounds, MaxBounds;
        public GameObject BallPrefab;
        [SerializeField] public GameObject[] Bricks;
        public float RespawnTimer;
        [HideInInspector] public bool GameRunning;
        [SerializeField] private GameObject _lossScreen;
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private TextMeshProUGUI _ballText;
        [SerializeField, Range(0, 15)] private int _maxRespawns;
        private int _respawnCount;

        void Start() {
            Instance = this;
            _lossScreen.SetActive(false);
            _winScreen.SetActive(false);
            GameRunning = true;
            Instantiate(BallPrefab);
            UpdateBallText();
        }

        public void BallOutOfBounds(GameObject ball) {
            Destroy(ball);
            _respawnCount += 1;
            UpdateBallText();
            if (_respawnCount < _maxRespawns) {
                StartCoroutine(WaitAndRespawn());
            } else {
                GameRunning = false;
                _lossScreen.SetActive(true);
            }
        }

        public void CheckForWin(GameObject ball) {
            foreach (GameObject brick in Bricks) {
                if (brick != null) return;
            }
            Destroy(ball);
            GameRunning = false;
            _winScreen.SetActive(true);
        }

        private IEnumerator WaitAndRespawn() {
            yield return new WaitForSeconds(RespawnTimer);
            Instantiate(BallPrefab);
        }

        private void UpdateBallText() {
            _ballText.text = "Balls Remaining: " + (_maxRespawns - _respawnCount) + "/" + _maxRespawns;
        }
    }
}
