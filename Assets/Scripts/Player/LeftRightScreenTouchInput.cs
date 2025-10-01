using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;


public class LeftRightScreenTouchInput : MonoBehaviour
{
    public static event Action OnTouchRight;
    public static event Action OnTouchLeft;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();       
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        {
            if (touch.isInProgress)
            {
                if (touch.screenPosition.x > Screen.width / 2f)
                    OnTouchRight?.Invoke();
                else
                    OnTouchLeft?.Invoke();
            }
        }
    }
}
