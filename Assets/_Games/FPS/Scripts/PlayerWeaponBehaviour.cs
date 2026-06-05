using UnityEngine;

namespace Games.FPS {
    public class PlayerWeaponBehaviour : WeaponBehaviour {
        void Update() {
            if (!Input.GetMouseButtonDown(0)) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            // Only fire when the ray actually hits something. The original
            // passed the (uninitialized) RaycastHit to FireWeapon
            // unconditionally — a missed shot would null-ref on the
            // hit.transform read inside WeaponBehaviour.
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit)) {
                FireWeapon(hit);
            }
        }
    }
}
