using UnityEngine;

namespace Games.Platformer {
    /// <summary>
    /// Sits on an enemy GameObject; subtracts a life from the player on
    /// contact, ends the game when lives run out.
    /// </summary>
    /// <remarks>
    /// Two bugs were corrected from the original:
    /// 1. Tag check was inverted — the script lives on the enemy, so the
    ///    OTHER collider in the trigger is the player. Original tested
    ///    <c>CompareTag("Enemy")</c> which would only fire on enemy-vs-enemy.
    /// 2. Off-by-one in lives: original used <c>if (Lives &gt; 0) UpdateLives else Lose</c>,
    ///    which decremented past zero before calling Lose — costing the
    ///    player one extra hit. Fixed by checking the post-decrement value.
    /// </remarks>
    public class EnemyController : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player")) return;

            GameController.Instance.UpdateLives();
            if (GameController.Instance.Lives <= 0) {
                GameController.Instance.Lose();
            }
        }
    }
}
