using UnityEngine;

namespace Games.Platformer {
    public class PlayerController : MonoBehaviour {
        public float speed = 5f;
        public float jumpForce = 5f;

        [Header("Ground check")]
        [Tooltip("Empty transform placed at the player's feet. Used to test for ground.")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask _groundLayers = ~0;

        private Rigidbody2D _rb;
        private Animator _animator;
        private bool _isGrounded;

        void Start() {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void Update() {
            // Ground check via a small overlap circle at the feet, rather
            // than collision-tag tracking. The original used OnCollisionEnter2D
            // with "Ground"/"Platform" tags, which (a) flipped isGrounded
            // true when touching a WALL too — enabling infinite wall-jumps —
            // and (b) needed paired enter/exit callbacks to stay accurate.
            // The overlap circle is one line and always correct.
            _isGrounded = _groundCheck != null
                && Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayers);

            float move = Input.GetAxis("Horizontal");
            _rb.linearVelocity = new Vector2(move * speed, _rb.linearVelocity.y);

            if (move != 0f) {
                transform.localScale = new Vector3(Mathf.Sign(move) * 0.5f, 0.5f, 1f);
            }

            // GetKeyDown so a single press = single jump. Original used
            // GetKey which would queue multiple jumps if Space was held
            // and isGrounded flickered true for a frame.
            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
                _rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }

            _animator.SetBool("isRunning", move != 0f);
            _animator.SetBool("isGrounded", _isGrounded);
        }

        // Visualize the ground-check radius in the editor.
        void OnDrawGizmosSelected() {
            if (_groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }
}
