using UnityEngine;

public class Boom : MonoBehaviour
{
    [Header("Warning Flash")]
    [SerializeField] private Color _warningColor = Color.red;
    [SerializeField] private float _countDown = 3f;
    [SerializeField] private float _minFlashHz = 1f;   // slow at start
    [SerializeField] private float _maxFlashHz = 8f;   // fast near zero

    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private float _remaining;
    private float _phase;
    private bool _exploded;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        _remaining = Mathf.Max(0f, _countDown);
        if (_spriteRenderer != null)
        {
            _baseColor = _spriteRenderer.color;
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
        // Implement your explosion logic here
        // e.g., play VFX/SFX, damage, and destroy gameObject
    }
}
