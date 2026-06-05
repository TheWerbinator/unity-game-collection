using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.Platformer {
    public class GameController : MonoBehaviour {
        public static GameController Instance;

        public int Lives = 3;
        public int Score = 0;

        [SerializeField] private GameObject _canvas;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private GameObject _lossScreen;
        [SerializeField] private GameObject _nextLevelScreen;
        [SerializeField] private GameObject _endWinScreen;
        [SerializeField] private AudioClip _winSound;
        [SerializeField] private AudioClip _lossSound;

        private int _levelScore;
        private bool _finalCheckpoint;
        private AudioSource _audioSource;

        void Awake() {
            // Singleton guard — every scene that contains a GameController
            // GameObject would otherwise spawn a duplicate on load. Keep
            // the original; destroy the new one before its Start runs.
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (_canvas != null) DontDestroyOnLoad(_canvas);
        }

        void Start() {
            _audioSource = GetComponent<AudioSource>();
            if (_lossScreen != null) _lossScreen.SetActive(false);
            if (_nextLevelScreen != null) _nextLevelScreen.SetActive(false);
            if (_endWinScreen != null) _endWinScreen.SetActive(!_finalCheckpoint == false);
            UpdateScoreText();
            UpdateLivesText();
        }

        public void UpdateScore() {
            Score++;
            _levelScore++;
            UpdateScoreText();
        }

        public void UpdateLives() {
            Lives--;
            UpdateLivesText();
        }

        public void NextLevel(bool isFinal) {
            _finalCheckpoint = isFinal;
            if (_nextLevelScreen != null) _nextLevelScreen.SetActive(true);
        }

        public void GoToNextLevel() {
            if (_finalCheckpoint) {
                SceneManager.LoadScene("Final");
                if (_endWinScreen != null) _endWinScreen.SetActive(true);
                // Hide the gameplay HUD on the final win screen — it's a
                // celebration, not active play. The win screen has its own
                // copy that shows the final total separately.
                SetHudVisible(false);
                if (_audioSource != null && _audioSource.isPlaying) _audioSource.Stop();
                if (_winSound != null) AudioSource.PlayClipAtPoint(_winSound, transform.position);
            } else {
                SceneManager.LoadScene("LevelTwo");
                if (_nextLevelScreen != null) _nextLevelScreen.SetActive(false);
                _levelScore = 0;
                UpdateScoreText();
            }
        }

        public void Lose() {
            if (_lossScreen != null) _lossScreen.SetActive(true);
            // Same reason as the win path: HUD is for active play; the loss
            // screen owns its own final-score display.
            SetHudVisible(false);
            if (_audioSource != null) _audioSource.Stop();
            if (_lossSound != null) AudioSource.PlayClipAtPoint(_lossSound, transform.position);
        }

        private void SetHudVisible(bool visible) {
            if (_scoreText != null) _scoreText.gameObject.SetActive(visible);
            if (_livesText != null) _livesText.gameObject.SetActive(visible);
        }

        private void UpdateScoreText() {
            if (_scoreText != null) _scoreText.text = $"Melons: {_levelScore}";
        }

        private void UpdateLivesText() {
            if (_livesText != null) _livesText.text = $"<3: {Lives}";
        }
    }
}
