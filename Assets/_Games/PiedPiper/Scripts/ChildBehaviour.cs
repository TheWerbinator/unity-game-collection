using System.Collections.Generic;
using UnityEngine;

namespace Games.PiedPiper {
    public class ChildBehaviour : MonoBehaviour {
        public bool Following = false;
        public Queue<Vector3> pathToFollow;
        [SerializeField] private float _followOffset = 1.5f;  // distance behind Piper for first follower
        [SerializeField] private float _spinSpeed = 90f;       // idle-state visual cue
        // After this many seconds of following, re-enable our collider so
        // the chain can wrap and bite itself. Children at positions 0, 1, 2
        // stay non-colliding so the player can't immediately lose by turning
        // around onto the just-recruited child.
        [SerializeField] private float _enableColliderDelay = 2f;
        [SerializeField] private float _scatterSpeed = 8f;
        private float _colliderTimer;
        private bool _firstStep = true;
        private bool _scattering;
        private Vector3 _scatterDir;

        /// <summary>
        /// Send this child fleeing in a random horizontal direction. They
        /// keep running until they cross the play-area boundary, then
        /// destroy themselves. Used by GameController.ProcessLoss for the
        /// "everyone flees the dead Piper" effect.
        /// </summary>
        public void StartScatter() {
            _scattering = true;
            Following = false;
            // Random direction on the XZ plane.
            float a = Random.Range(0f, Mathf.PI * 2f);
            _scatterDir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
        }

        void Start() {
            _colliderTimer = _enableColliderDelay;
        }

        void Update() {
            if (_scattering) {
                transform.position += _scatterDir * (_scatterSpeed * Time.deltaTime);
                // Use a slightly larger bound than the gameplay area so the
                // child visibly clears the wall before disappearing.
                Vector3 p = transform.position;
                if (p.x > 22f || p.x < -22f || p.z > 12f || p.z < -12f) {
                    Destroy(gameObject);
                }
                return;
            }

            if (!Following) {
                transform.Rotate(Vector3.up, _spinSpeed * Time.deltaTime);
                return;
            }

            if (_firstStep) {
                AudioSource src = GetComponent<AudioSource>();
                if (src != null) src.Play();
            }

            // Guard against an empty queue: the follower can catch up to
            // its leader's recorded positions faster than the leader writes
            // new ones (especially right after pickup, where Piper.Path is
            // cleared). Peek throws on empty — skip the frame instead.
            if (pathToFollow == null || pathToFollow.Count == 0) return;

            Vector3 nextPosition = pathToFollow.Peek();

            if (_firstStep) {
                if (PiperBehaviour.Instance.children.Count != 0) {
                    transform.position = nextPosition;
                } else {
                    transform.position = PiperBehaviour.Instance.transform.position
                                         - PiperBehaviour.Instance.transform.forward * _followOffset;
                }
                _firstStep = false;
            } else {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    nextPosition,
                    GameController.Instance.MovementSpeed * Time.deltaTime
                );
            }

            // Re-enable collider after delay, only for children far enough
            // back in the chain that the Piper can't trivially run into them.
            if (!_firstStep && _colliderTimer > 0
                && PiperBehaviour.Instance.children.IndexOf(transform) > 2) {
                _colliderTimer -= Time.deltaTime;
                if (_colliderTimer <= 0) {
                    Collider col = GetComponent<Collider>();
                    if (col != null) col.enabled = true;
                }
            }

            if (Vector3.Distance(transform.position, nextPosition) < 0.001f) {
                pathToFollow.Dequeue();
            }
        }
    }
}
