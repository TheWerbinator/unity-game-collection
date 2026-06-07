using UnityEngine;

namespace Games.DoomClone
{
  public class DoorController : MonoBehaviour
  {
    public float openHeight = 3f;
    public float openSpeed = 2f;
    public float closeDelay = 3f;
    public float activationRange = 3f;

    [Header("Sounds")]
    [Tooltip("Played when the door starts opening.")]
    public AudioClip openClip;
    [Tooltip("Played when the door starts closing.")]
    public AudioClip closeClip;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpening;
    private bool _isClosing;
    private AudioSource _audio;

    void Start()
    {
      Debug.Log($"DoorController started on {name}");
      _closedPosition = transform.position;
      _openPosition = _closedPosition + new Vector3(0f, openHeight, 0f);
      _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
      if (Input.GetKeyDown(KeyCode.Space))
      {
        float dist = PlayerBehaviour.Instance != null
          ? Vector3.Distance(transform.position, PlayerBehaviour.Instance.transform.position)
          : -1f;
        Debug.Log($"[{name}] Space pressed. dist={dist:F2} range={activationRange} nearby={IsPlayerNearby()} opening={_isOpening} closing={_isClosing}");
      }
      if (Input.GetKeyDown(KeyCode.Space) && IsPlayerNearby() && !_isOpening && !_isClosing)
      {
        PlayClip(openClip);
        // Original wrapped a single bool assignment in a coroutine
        // that yielded once and did nothing useful. Direct.
        _isOpening = true;
      }

      if (_isOpening)
      {
        transform.position = Vector3.MoveTowards(transform.position, _openPosition, openSpeed * Time.deltaTime);
        if (transform.position == _openPosition)
        {
          _isOpening = false;
          Invoke(nameof(StartClosing), closeDelay);
        }
      }
      else if (_isClosing)
      {
        transform.position = Vector3.MoveTowards(transform.position, _closedPosition, openSpeed * Time.deltaTime);
        if (transform.position == _closedPosition)
        {
          _isClosing = false;
        }
      }
    }

    private void PlayClip(AudioClip clip)
    {
      if (_audio == null || clip == null) return;
      _audio.PlayOneShot(clip);
    }

    private bool IsPlayerNearby()
    {
      if (PlayerBehaviour.Instance == null) return false;
      // Measure distance to the closest point on the door's renderer bounds
      // rather than to the GameObject's pivot. ProBuilder geometry inherited
      // from larger meshes often has a pivot far away from the visible door
      // (e.g. at the original wall's pivot), which makes a pivot-based
      // distance check unusable. ClosestPoint gives the nearest point on the
      // door's actual bounding box regardless of where the pivot sits.
      Renderer rend = GetComponent<Renderer>();
      Vector3 playerPos = PlayerBehaviour.Instance.transform.position;
      Vector3 sample = rend != null ? rend.bounds.ClosestPoint(playerPos) : transform.position;
      float r2 = activationRange * activationRange;
      return (sample - playerPos).sqrMagnitude < r2;
    }

    private void StartClosing()
    {
      _isClosing = true;
      PlayClip(closeClip);
    }
  }
}
