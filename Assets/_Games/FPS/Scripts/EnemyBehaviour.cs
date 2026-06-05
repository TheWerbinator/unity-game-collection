namespace Games.FPS {
    public class EnemyBehaviour : CharacterBehaviour {
        public override void Die() {
            // Tip the body forward as a death animation stand-in.
            transform.Rotate(-75f, 0f, 0f);
            EnemyMovementBehaviour move = GetComponent<EnemyMovementBehaviour>();
            if (move != null) move.enabled = false;
            EnemyWeaponBehaviour weapon = GetComponent<EnemyWeaponBehaviour>();
            if (weapon != null) weapon.enabled = false;
        }
    }
}
