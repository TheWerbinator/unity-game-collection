using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.Shared {
    /// <summary>
    /// Lives on a GameObject in MainMenu, persists across scene loads via
    /// DontDestroyOnLoad. Two responsibilities:
    /// 1. Drive scene transitions when the menu's UI buttons are clicked
    ///    (each button's OnClick fires <see cref="LoadScene"/>).
    /// 2. Listen for Escape in every scene and bounce back to MainMenu.
    ///
    /// The singleton guard means duplicate loaders from re-entering MainMenu
    /// don't pile up.
    /// </summary>
    public class SceneLoader : MonoBehaviour {
        public static SceneLoader Instance;

        /// <summary>Name of the menu scene. Must match the scene's filename.</summary>
        public const string MainMenuScene = "MainMenu";

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update() {
            // Esc → menu, unless we're already in the menu.
            if (Input.GetKeyDown(KeyCode.Escape)
                && SceneManager.GetActiveScene().name != MainMenuScene) {
                LoadScene(MainMenuScene);
            }
        }

        /// <summary>
        /// Wire this method from each menu Button's OnClick event. Pass the
        /// scene's filename (without ".unity") as the string argument.
        /// </summary>
        public void LoadScene(string sceneName) {
            if (string.IsNullOrEmpty(sceneName)) return;
            // FPS / DoomClone lock the cursor on Start; release it before
            // loading so the menu remains clickable after returning, and so
            // the OS cursor reappears immediately.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>Wire to a Quit button. No-op in WebGL builds; works on standalone.</summary>
        public void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
