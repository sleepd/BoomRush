using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIRestart : MonoBehaviour
{
    InputSystem_Actions inputActions;
    void Awake()
    {
        inputActions = new();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Jump.performed += Restart;
    }

    private void Restart(InputAction.CallbackContext context)
    {
        OnDisable();
        GameManager.Instance.Restart();
    }

    void OnDisable()
    {
        inputActions.Player.Jump.performed -= Restart;
        inputActions.Disable();
    }
}
