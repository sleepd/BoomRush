using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool normalizeDiagonal = true;
    [SerializeField] GameObject _wallPrefab;
    [SerializeField] int _wallLimit = 3;

    private InputSystem_Actions inputAction;
    private InputAction _jumpAction;
    private readonly List<GameObject> _spawnedWalls = new();

    private void Awake()
    {
        inputAction = new InputSystem_Actions();
        _jumpAction = inputAction.asset.FindAction("Player/Jump", throwIfNotFound: false);
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
    }

    private void Update()
    {
        Vector2 move = Vector2.zero;
        if (inputAction != null)
        {
            // Assumes a "Move" action in the "Player" action map
            move = inputAction.Player.Move.ReadValue<Vector2>();
        }

        if (normalizeDiagonal && move.sqrMagnitude > 1f)
        {
            move = move.normalized;
        }

        Vector3 delta = new Vector3(move.x, move.y, 0f) * (moveSpeed * Time.deltaTime);
        transform.position += delta;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        TrySpawnWall();
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
}
