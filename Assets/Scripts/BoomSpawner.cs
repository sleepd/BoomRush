using UnityEngine;

public class BoomSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _boomPrefab;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _minPerBatch = 1;
    [SerializeField] private int _maxPerBatch = 3;

    [Header("Area (centered on this transform)")]
    [SerializeField] private float _width = 19.20f;
    [SerializeField] private float _height = 10.80f;

    private float _timer;
    private int _currentBatchSize;

    private void OnEnable()
    {
        _timer = 1f;
        _currentBatchSize = Mathf.Clamp(_minPerBatch, 0, _maxPerBatch);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsRunning) return;
        
        if (_boomPrefab == null)
            return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            PlayerController.Instance.ResetWalls();
            SpawnBatch();
            _timer = Mathf.Max(0.01f, _spawnInterval);
        }
    }

    private void SpawnBatch()
    {
        int count = Mathf.Clamp(_currentBatchSize, 0, 1000);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = RandomEdgePosition();
            Instantiate(_boomPrefab, pos, Quaternion.identity);
        }

        if (_currentBatchSize < _maxPerBatch)
        {
            _currentBatchSize = Mathf.Min(_currentBatchSize + 1, _maxPerBatch);
        }
    }

    private Vector3 RandomEdgePosition()
    {
        float halfW = Mathf.Max(0f, _width) * 0.5f;
        float halfH = Mathf.Max(0f, _height) * 0.5f;
        Vector3 center = transform.position;

        // 0: left, 1: right, 2: bottom, 3: top
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // left edge x = -halfW
                return new Vector3(center.x - halfW, center.y + Random.Range(-halfH, halfH), 0f);
            case 1: // right edge x = +halfW
                return new Vector3(center.x + halfW, center.y + Random.Range(-halfH, halfH), 0f);
            case 2: // bottom edge y = -halfH
                return new Vector3(center.x + Random.Range(-halfW, halfW), center.y - halfH, 0f);
            case 3: // top edge y = +halfH
            default:
                return new Vector3(center.x + Random.Range(-halfW, halfW), center.y + halfH, 0f);
        }
    }

    private void OnValidate()
    {
        if (_maxPerBatch < _minPerBatch) _maxPerBatch = _minPerBatch;
        if (_spawnInterval < 0f) _spawnInterval = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Vector3 size = new Vector3(Mathf.Max(0f, _width), Mathf.Max(0f, _height), 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
