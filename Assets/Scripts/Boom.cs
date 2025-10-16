using UnityEngine;

public class Boom : MonoBehaviour
{
    [Header("Warning Flash")]
    [SerializeField] private Color _warningColor = Color.red;
    [SerializeField] private float _countDown = 3f;
    [SerializeField] private float _minFlashHz = 1f;   // slow at start
    [SerializeField] private float _maxFlashHz = 8f;   // fast near zero
    [Header("Explosion")]
    [SerializeField] private LayerMask _lineOfSightMask = Physics2D.DefaultRaycastLayers;

    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private Color _initialColor;
    private bool _initialColorCached;
    private float _remaining;
    private float _phase;
    private bool _exploded;
    private Collider2D _selfCollider;
    private Collider2D _playerCollider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        CacheInitialColor();
        _selfCollider = GetComponent<Collider2D>();
        CachePlayerCollider();
    }

    private void OnEnable()
    {
        _exploded = false;
        _phase = -0.5f * Mathf.PI; // start sine wave at base color
        _remaining = Mathf.Max(0f, _countDown);
        if (_spriteRenderer != null)
        {
            CacheInitialColor();
            _baseColor = _initialColor;
            _spriteRenderer.color = _baseColor;
        }

        if (_remaining <= 0f)
        {
            TriggerExplosionOnce();
        }
    }

    private void Update()
    {
        if (_exploded)
            return;

        if (_remaining > 0f)
        {
            _remaining -= Time.deltaTime;

            float duration = Mathf.Max(0.0001f, _countDown);
            float progress = 1f - (_remaining / duration); // 0 -> 1 as time passes
            float freq = Mathf.Lerp(_minFlashHz, _maxFlashHz, progress);

            _phase += 2f * Mathf.PI * freq * Time.deltaTime;
            float t = (Mathf.Sin(_phase) + 1f) * 0.5f; // 0..1

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.Lerp(_baseColor, _warningColor, t);
            }
        }

        if (_remaining <= 0f)
        {
            TriggerExplosionOnce();
        }
    }

    private void TriggerExplosionOnce()
    {
        if (_exploded)
            return;
        _exploded = true;
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _warningColor;
        }
        Explosion();
    }

    private void Explosion()
    {


        if (_playerCollider == null)
        {
            CachePlayerCollider();
        }

        bool hasLineOfSight = HasLineOfSightToPlayer();
        if (!hasLineOfSight)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.Gameover();
    }

    private bool HasLineOfSightToPlayer()
    {
        Vector2 origin = transform.position;
        Vector2 target = PlayerController.Instance.transform.position;

        RaycastHit2D[] hits = Physics2D.LinecastAll(origin, target, _lineOfSightMask);
        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            if (_selfCollider != null && hit.collider == _selfCollider)
                continue;

            if (IsPlayerCollider(hit.collider))
                return true;

            // First meaningful hit is not the player, so the path is blocked.
            return false;
        }

        // Nothing hit along the line; treat as unobstructed.
        return true;
    }

    private bool IsPlayerCollider(Collider2D candidate)
    {
        if (candidate == null || PlayerController.Instance == null)
            return false;

        if (_playerCollider != null && candidate == _playerCollider)
            return true;

        return candidate.transform == PlayerController.Instance || candidate.transform.IsChildOf(PlayerController.Instance.transform);
    }

    private void CachePlayerCollider()
    {
        _playerCollider = null;
        _playerCollider = PlayerController.Instance.GetComponent<Collider2D>();
    }

    private void CacheInitialColor()
    {
        if (_spriteRenderer == null || _initialColorCached)
            return;

        _initialColor = _spriteRenderer.color;
        _baseColor = _initialColor;
        _initialColorCached = true;
    }
}
