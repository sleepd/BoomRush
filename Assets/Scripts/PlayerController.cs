using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool normalizeDiagonal = true;
    [SerializeField] GameObject _wallPrefab;
    [SerializeField] int _wallLimit = 3;
    [SerializeField] private Vector2 movementBoundsSize = new(19f, 10f);

    private Rigidbody2D _rigidbody;
    private InputSystem_Actions inputAction;
    private InputAction _jumpAction;
    private Vector2 _moveInput;
    private readonly List<GameObject> _spawnedWalls = new();
    private Vector2 _movementBoundsCenter;

    public override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogWarning($"{nameof(PlayerController)} requires a {nameof(Rigidbody2D)} component.");
        }

        inputAction = new InputSystem_Actions();
        _jumpAction = inputAction.asset.FindAction("Player/Jump", throwIfNotFound: false);
        _movementBoundsCenter = transform.position;
    }

    private void OnEnable()
    {
        inputAction.Enable();
        if (_jumpAction != null)
        {
            _jumpAction.performed += OnJumpPerformed;
        }
    }

    private void OnDisable()
    {
        if (_jumpAction != null)
        {
            _jumpAction.performed -= OnJumpPerformed;
        }
        inputAction.Disable();
        _moveInput = Vector2.zero;
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsRunning)
        {
            _moveInput = Vector2.zero;
            return;
        }

        if (inputAction != null)
        {
            // Assumes a "Move" action in the "Player" action map
            _moveInput = inputAction.Player.Move.ReadValue<Vector2>();
        }
        else
        {
            _moveInput = Vector2.zero;
        }

        if (normalizeDiagonal && _moveInput.sqrMagnitude > 1f)
        {
            _moveInput = _moveInput.normalized;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        TrySpawnWall();
    }

    private void FixedUpdate()
    {
        if (_rigidbody == null)
            return;

        Vector2 delta = _moveInput * (moveSpeed * Time.fixedDeltaTime);
        Vector2 targetPosition = _rigidbody.position + delta;

        if (movementBoundsSize.x > 0f && movementBoundsSize.y > 0f)
        {
            // Keep the player within the rectangular movement bounds centered on spawn position.
            Vector2 halfSize = movementBoundsSize * 0.5f;
            float minX = _movementBoundsCenter.x - halfSize.x;
            float maxX = _movementBoundsCenter.x + halfSize.x;
            float minY = _movementBoundsCenter.y - halfSize.y;
            float maxY = _movementBoundsCenter.y + halfSize.y;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        _rigidbody.MovePosition(targetPosition);
    }

    private void TrySpawnWall()
    {
        if (_wallPrefab == null)
        {
            Debug.LogWarning("PlayerController: _wallPrefab is not assigned.");
            return;
        }

        _spawnedWalls.RemoveAll(w => w == null);
        if (_spawnedWalls.Count >= _wallLimit)
        {
            return;
        }

        var go = Instantiate(_wallPrefab, transform.position, Quaternion.identity);
        _spawnedWalls.Add(go);
    }

    public void ResetWalls()
    {
        for (int i = 0; i < _spawnedWalls.Count; i++)
        {
            if (_spawnedWalls[i] != null)
            {
                Destroy(_spawnedWalls[i]);
            }
        }
        _spawnedWalls.Clear();
    }

    public void Die()
    {
        if (_jumpAction != null)
        {
            _jumpAction.performed -= OnJumpPerformed;
        }
        inputAction.Disable();
        _moveInput = Vector2.zero;
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }
    }
}
