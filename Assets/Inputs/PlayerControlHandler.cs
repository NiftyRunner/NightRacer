using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlHandler : MonoBehaviour
{
    private PlayerControls playerInputActions;

    public static event Action<float> OnLRValueChange;

    private void Awake()
    {
        playerInputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.Player.LR.performed += LR_performed;
        playerInputActions.Player.LR.canceled += LR_canceled;
    }


    private void OnDisable()
    {
        playerInputActions.Disable();

        playerInputActions.Player.LR.performed -= LR_performed;
        playerInputActions.Player.LR.canceled -= LR_canceled;
    }

    private void LR_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Lr performed");
        float inputValues = obj.ReadValue<float>();
        Debug.Log(inputValues);
        OnLRValueChange?.Invoke(inputValues);
    }
    private void LR_canceled(InputAction.CallbackContext obj)
    {
        OnLRValueChange?.Invoke(0);
    }
}
