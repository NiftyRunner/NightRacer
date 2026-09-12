using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlHandler : MonoBehaviour
{
    private PlayerControls playerInputActions;

    public static event Action<float> OnLRValueChange;
    public static event Action OnEnterPressed;
    public static event Action OnEscPressed;

    private void Awake()
    {
        playerInputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        if (playerInputActions == null) playerInputActions = new PlayerControls();

        playerInputActions.Enable();

        playerInputActions.Player.LR.performed += LR_performed;
        playerInputActions.Player.LR.canceled += LR_canceled;
        playerInputActions.Player.Enter.performed += Enter_performed;
        playerInputActions.Player.Escape.performed += Escape_performed;
    }


    private void OnDisable()
    {
        if (playerInputActions == null) return;

        playerInputActions.Disable();

        playerInputActions.Player.LR.performed -= LR_performed;
        playerInputActions.Player.LR.canceled -= LR_canceled;
        playerInputActions.Player.Enter.performed -= Enter_performed;
        playerInputActions.Player.Escape.performed -= Escape_performed;
    }

    private void Enter_performed(InputAction.CallbackContext obj)
    {
        OnEnterPressed?.Invoke();
    }
    private void Escape_performed(InputAction.CallbackContext obj)
    {
        OnEscPressed?.Invoke();
    }

    private void LR_performed(InputAction.CallbackContext obj)
    {
        float inputValues = obj.ReadValue<float>();
        OnLRValueChange?.Invoke(inputValues);
    }
    private void LR_canceled(InputAction.CallbackContext obj)
    {
        OnLRValueChange?.Invoke(0);
    }
}
