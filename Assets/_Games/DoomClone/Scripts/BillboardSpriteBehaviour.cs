using UnityEngine;
using UnityEngine.AI;

namespace Games.DoomClone {
    /// <summary>
    /// Classic Doom-style billboard: a 2D sprite that always faces the
    /// camera, with three sprite variants (front, side, back) selected
    /// by the angle between the *enemy's walking direction* and the
    /// *direction from enemy to player*. Side sprite flips horizontally
    /// to serve both left- and right-facing angles.
    /// </summary>
    /// <remarks>
    /// Two bugs in earlier versions:
    /// (1) original computed the angle off this transform's forward, but
    ///     this transform is the billboard itself — it always faces the
    ///     camera, so the angle was always ~180° → only the back sprite
    ///     ever showed.
    /// (2) the script needs the enemy's *logical* facing, not its billboard
    ///     facing. We read that from the NavMeshAgent's velocity (the AI
    ///     it's parented to). When velocity is near zero, we hold the
    ///     last-known facing so a stationary enemy doesn't strobe.
    ///
    /// Expected hierarchy: this script lives on a child GameObject (a
    /// quad / sprite). The NavMeshAgent + AI live on the parent.
    /// </remarks>
    public class BillboardSpriteBehaviour : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
        public Sprite frontSprite;
        public Sprite sideSprite;
        public Sprite backSprite;

        private NavMeshAgent _agent;
        private Vector3 _lastFacing = Vector3.forward;

        void Start() {
            _agent = GetComponentInParent<NavMeshAgent>();
        }

        void Update() {
            if (PlayerBehaviour.Instance == null || spriteRenderer == null) return;

            // 1. Billboard: rotate the sprite so it faces the player camera.
            //    Flatten Y so the sprite stays upright instead of tilting.
            Vector3 toPlayer = PlayerBehaviour.Instance.transform.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude > 0.0001f) {
                transform.rotation = Quaternion.LookRotation(-toPlayer);
            }

            // 2. Enemy's logical facing — from NavMeshAgent velocity. Hold
            //    last-known facing if currently stationary so the sprite
            //    doesn't flip unpredictably on idle frames.
            Vector3 facing = _lastFacing;
            if (_agent != null) {
                Vector3 v = _agent.velocity;
                v.y = 0f;
                if (v.sqrMagnitude > 0.01f) {
                    facing = v.normalized;
                    _lastFacing = facing;
                }
            }

            // 3. Choose sprite by angle between enemy's facing and the
            //    direction from enemy to player.
            //    ~0° → enemy facing player → front sprite (chase mode)
            //    ~±90° → enemy facing perpendicular → side sprite
            //    ~180° → enemy facing away from player → back sprite
            Vector3 enemyToPlayer = toPlayer.normalized;
            float angle = Vector3.SignedAngle(facing, enemyToPlayer, Vector3.up);
            float abs = Mathf.Abs(angle);

            if (abs < 45f) {
                spriteRenderer.sprite = frontSprite;
                spriteRenderer.flipX = false;
            } else if (abs > 135f) {
                spriteRenderer.sprite = backSprite;
                spriteRenderer.flipX = false;
            } else if (angle > 0f) {
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.flipX = true;
            }
        }
    }
}
