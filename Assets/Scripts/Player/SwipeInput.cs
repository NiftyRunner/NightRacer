using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// A robust, persistent, and event-driven input manager for handling taps, holds, and swipes.
/// This script uses the Enhanced Touch API and tracks a single finger to avoid multi-touch conflicts.
/// </summary>
public class SwipeInput : MonoBehaviour
{
    // Event for continuous hold value (-1 for left, 1 for right, 0 for none).
    public static event Action<float> OnTouchValueChange;

    // Discrete event for a tap-and-release on the right side.
    public static event Action OnTapRight;

    // Discrete event for a tap-and-release on the left side.
    public static event Action OnTapLeft;

    // Discrete event for a left swipe.
    public static event Action OnSwipeLeft;

    // Discrete event for a right swipe.
    public static event Action OnSwipeRight;

    [Tooltip("The minimum distance in screen pixels a swipe must travel to be recognized.")]
    [SerializeField] private float minSwipeDistance = 100f;

    [Tooltip("The maximum distance a finger can move and still be considered a tap.")]
    [SerializeField] private float maxTapDistance = 50f;

    // We will track one specific finger to avoid multi-touch issues.
    private Finger trackingFinger;
    private Vector2 startPosition;
    private bool isSwipeDetected;

    private void Awake()
    {
        // Make this a persistent singleton
        if (FindObjectsByType<SwipeInput>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.onFingerDown += HandleFingerDown;
        ETouch.onFingerMove += HandleFingerMove;
        ETouch.onFingerUp += HandleFingerUp;
    }

    private void OnDisable()
    {
        ETouch.onFingerDown -= HandleFingerDown;
        ETouch.onFingerMove -= HandleFingerMove;
        ETouch.onFingerUp -= HandleFingerUp;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerDown(Finger finger)
    {
        // If we are already tracking a finger, ignore new touches.
        if (trackingFinger != null) return;

        trackingFinger = finger;
        startPosition = finger.screenPosition;
        isSwipeDetected = false;

        // Immediately determine hold side and invoke the continuous event.
        if (finger.screenPosition.x > Screen.width / 2f)
        {
            OnTouchValueChange?.Invoke(1f);
        }
        else
        {
            OnTouchValueChange?.Invoke(-1f);
        }
    }

    private void HandleFingerMove(Finger finger)
    {
        // Only process movement for the finger we are tracking.
        if (finger != trackingFinger) return;

        Vector2 delta = finger.screenPosition - startPosition;

        // Check for a swipe only if one hasn't been detected yet for this touch.
        // This prevents re-classifying a swipe as a tap if the user moves back.
        if (!isSwipeDetected)
        {
            // A swipe is detected if it's moved far enough horizontally
            // and the horizontal movement is greater than the vertical movement.
            if (Mathf.Abs(delta.x) > minSwipeDistance && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                isSwipeDetected = true;
            }
        }
    }

    private void HandleFingerUp(Finger finger)
    {
        if (finger != trackingFinger) return;

        float distanceMoved = Vector2.Distance(finger.screenPosition, startPosition);

        // A. Check for a SWIPE first. This has priority.
        // isSwipeDetected is true if it moved far enough HORIZONTALLY in HandleFingerMove.
        if (isSwipeDetected)
        {
            Vector2 swipeVector = finger.screenPosition - startPosition;
            if (swipeVector.x > 0)
            {
                OnSwipeRight?.Invoke();
            }
            else
            {
                OnSwipeLeft?.Invoke();
            }
        }
        // B. If not a swipe, check if it was a TAP.
        // A tap is a touch that moved less than our maximum tap distance.
        else if (distanceMoved < maxTapDistance)
        {
            if (finger.screenPosition.x > Screen.width / 2f)
            {
                OnTapRight?.Invoke();
            }
            else
            {
                OnTapLeft?.Invoke();
            }
        }
        // C. If it was neither a swipe nor a tap (e.g., a slow drag), we do nothing.

        // Reset state for the next touch.
        OnTouchValueChange?.Invoke(0f);
        trackingFinger = null;
        isSwipeDetected = false;
    }
}