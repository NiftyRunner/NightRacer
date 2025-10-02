using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class LeftRightScreenTouchInput : MonoBehaviour
{
    public static event Action<float> OnTouchValueChange;
    public static event Action OnTouchRight;
    public static event Action OnTouchLeft;

    private float touchInputValue;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        // Don’t disable globally here unless this is your only input handler
        // EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        float newValue = 0f;

        // Take the *first active touch* (or the most recent one)
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

            if (touch.isInProgress)
            {
                if (touch.screenPosition.x > Screen.width / 2f)
                {
                    newValue = 1f;
                    if (touchInputValue != 1f) OnTouchRight?.Invoke();
                }
                else
                {
                    newValue = -1f;
                    if (touchInputValue != -1f) OnTouchLeft?.Invoke();
                }
            }
        }

        // Only fire event if value changed
        if (Math.Abs(newValue - touchInputValue) > 0.01f)
        {
            touchInputValue = newValue;
            OnTouchValueChange?.Invoke(touchInputValue);
        }
    }
}
