using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.PiedPiper {
    public class PiperBehaviour : MonoBehaviour {
        public List<Transform> children = new List<Transform>();
        public static PiperBehaviour Instance;
        [SerializeField] private GameObject _startScreen;
        [SerializeField] private TextMeshProUGUI _scoreUIText;
        private Rigidbody _rigidBody;
        private float _points = 100f;

        void Start() {
            Instance = this;
            _rigidBody = GetComponent<Rigidbody>();
            _startScreen.SetActive(true);
            _scoreUIText.text = "";
        }

        void Update() {
            if (!_startScreen.activeSelf && !GameController.Instance.Lose) {
                _points -= Time.deltaTime;
                _scoreUIText.text = "Points: " + _points.ToString("F0");
            }
        }

        void FixedUpdate() {
            // Out-of-bounds check first so we don't keep applying velocity
            // after we've already lost.
            Vector3 pos = transform.position;
            if (pos.x > 20f || pos.x < -20f || pos.z > 10f || pos.z < -10f) {
                GameController.Instance.Lose = true;
            }

            // Start the game on first directional keypress.
            bool anyDirKey = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                             Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
            if (anyDirKey && _startScreen.activeSelf) {
                GameController.Instance.GetComponent<AudioSource>().Play();
                _startScreen.SetActive(false);
                GameController.Instance.NeedNewChild = true;
            }

            if (GameController.Instance.Lose) {
                _rigidBody.linearVelocity = Vector3.zero;
                return;
            }

            // Combine inputs for diagonal movement; normalize so diagonals
            // don't move faster than cardinal directions. The original used
            // an if/else chain that picked only one direction per frame.
            Vector3 input = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) input += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) input += Vector3.back;
            if (Input.GetKey(KeyCode.A)) input += Vector3.left;
            if (Input.GetKey(KeyCode.D)) input += Vector3.right;
            _rigidBody.linearVelocity = input.normalized * GameController.Instance.MovementSpeed;
        }

        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Child")) return;
            ChildBehaviour child = other.GetComponent<ChildBehaviour>();
            if (child == null) return;

            if (child.Following) {
                // Hit our own chain — game over.
                GameController.Instance.Lose = true;
                return;
            }

            // Free child — pick it up.
            child.Following = true;
            other.enabled = false;
            children.Add(other.transform);
            _points += 5;
            GameController.Instance.NeedNewChild = true;
            GameController.Instance.RecordChildPickup();

            // Hand the new child the path it should follow. The first child
            // follows the Piper's recorded path; subsequent children follow
            // the path of the child in front of them in the chain.
            PathRecorder source = children.Count == 1
                ? Instance.GetComponent<PathRecorder>()
                : children[children.Count - 2].GetComponent<PathRecorder>();
            child.pathToFollow = source.Path;
            source.Path.Clear();
        }
    }
}
