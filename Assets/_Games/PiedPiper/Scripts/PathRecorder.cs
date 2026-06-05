using System.Collections.Generic;
using UnityEngine;

namespace Games.PiedPiper {
    /// <summary>
    /// Records this object's transform position into a queue at a fixed
    /// interval so that a follower can dequeue and play back the path.
    /// </summary>
    /// <remarks>
    /// The original recordInterval was 0.001s (1 ms), which queued ~1000
    /// positions per second per recorder and grew the heap unboundedly on
    /// long sessions. 0.05s (20 Hz) is plenty smooth for the follower's
    /// MoveTowards interpolation and is 50× less memory pressure.
    /// </remarks>
    public class PathRecorder : MonoBehaviour {
        public Queue<Vector3> Path = new Queue<Vector3>();
        public float recordInterval = 0.05f;
        private float _timer;

        void Update() {
            _timer += Time.deltaTime;
            if (_timer >= recordInterval) {
                Path.Enqueue(transform.position);
                _timer = 0f;
            }
        }
    }
}
