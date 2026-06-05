using UnityEngine;

namespace Games.FPS {
    /// <summary>
    /// Mouse-look. The camera owns horizontal yaw on the player body and
    /// vertical pitch on itself — coupling the body's facing to mouse X is
    /// the standard FPS convention so movement-forward matches look-forward.
    /// </summary>
    public class CameraController : MonoBehaviour {
        [SerializeField] private GameObject _player;
        [SerializeField] private float _minVerticalRotation = -80f;
        [SerializeField] private float _maxVerticalRotation = 80f;
        [SerializeField] private float _rotationSensitivity = 100f;
        [SerializeField] private float _maxRotationPerFrame = 90f;

        private Vector3 _horizontalRotation;
        private Vector3 _verticalRotation;

        private void Update() {
            Vector3 horizontalDelta = new Vector3(0f, Input.GetAxis("Mouse X") * Time.deltaTime * _rotationSensitivity);
            Vector3 verticalDelta = new Vector3(-Input.GetAxis("Mouse Y") * Time.deltaTime * _rotationSensitivity, 0f);

            _horizontalRotation += Vector3.ClampMagnitude(horizontalDelta, _maxRotationPerFrame);
            _verticalRotation += Vector3.ClampMagnitude(verticalDelta, _maxRotationPerFrame);

            _verticalRotation.x = Mathf.Clamp(_verticalRotation.x, _minVerticalRotation, _maxVerticalRotation);

            if (_player != null) {
                _player.transform.eulerAngles = _horizontalRotation;
            }
            transform.eulerAngles = new Vector3(_verticalRotation.x, transform.eulerAngles.y, 0f);
        }
    }
}
