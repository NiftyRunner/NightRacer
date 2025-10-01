using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class SwipeInput : MonoBehaviour
{
    public static event Action OnSwipeLeft;
    public static event Action OnSwipeRight;

    [SerializeField] private float minSwipeDistance = 100f;

    private Vector2 startPos;
    private bool isSwiping = false;

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
        foreach(var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        {
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    startPos = touch.screenPosition;
                    isSwiping = true;
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    if (!isSwiping) return;
                    Vector2 endPos = touch.screenPosition;
                    Vector2 delta = endPos - startPos;
                    
                    if(Mathf.Abs(delta.x) >minSwipeDistance && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        if (delta.x > 0) OnSwipeRight?.Invoke();
                        else OnSwipeLeft?.Invoke();
                    }
                    isSwiping = false;
                    break;
            }
        }   
    }
}
