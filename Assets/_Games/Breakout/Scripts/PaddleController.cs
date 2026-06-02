using UnityEngine;

namespace Games.Breakout {
    public class PaddleController : MonoBehaviour {
        public static PaddleController Instance;
        [SerializeField] private Vector3 _paddlePosition, _startScale;
        public float Speed;

        void Start() {
            Instance = this;
            transform.position = _paddlePosition;
            transform.localScale = _startScale;
        }

        void Update() {
            if (!GameManager.Instance.GameRunning) return;
            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x + _startScale.x / 2f > GameManager.Instance.MinBounds.position.x)
                transform.position += Vector3.left * Time.deltaTime * Speed;
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x - _startScale.x / 2f < GameManager.Instance.MaxBounds.position.x)
                transform.position += Vector3.right * Time.deltaTime * Speed;
        }

        public void ResetPosition() => transform.position = _paddlePosition;
    }
}
