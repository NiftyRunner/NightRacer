using UnityEngine;
using Cinemachine;
using System;

public class DollyDrive : MonoBehaviour
{
    public static event Action OnDollyFinished;

    [Header("Dolly Movement")]
    public float speed = 5f;

    // Your spline-track cart component
    private CinemachineDollyCart dollyCart;

    [Header("Cinemachine Cams")]
    [Tooltip("The vcam riding on the spline")]
    public CinemachineVirtualCamera vcamSpline;
    [Tooltip("The vcam that follows the player")]
    public CinemachineVirtualCamera vcamPlayer;

    [Header("Blend Settings")]
    [Tooltip("How long the blend lasts (seconds)")]
    public float blendDuration = 1.5f;

    private CinemachineBrain cinemachineBrain;
    private float trackLength;

    void Awake()
    {
        dollyCart = GetComponent<CinemachineDollyCart>();
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

        // Set initial priorities
        vcamSpline.Priority = 10;
        vcamPlayer.Priority = 0;

        // Configure the default blend time
        cinemachineBrain.m_DefaultBlend.m_Time = blendDuration;

        // Grab the end of the track
        var path = dollyCart.m_Path;
        trackLength = (path is CinemachineSmoothPath smooth)
            ? smooth.PathLength
            : 1f; // if PathUnits = PathUnits.Normalized, length is 1
    }

    void Update()
    {
        // Move the cart along the track
        dollyCart.m_Position += speed * Time.deltaTime;

        // Check if we’ve reached (or passed) the end
        if (dollyCart.m_Position >= trackLength)
        {
            TriggerCameraSwitch();
            // Optionally clamp so you don’t repeatedly trigger:
            dollyCart.m_Position = trackLength;
            enabled = false;  // stop updating if you don’t need further movement
            OnDollyFinished?.Invoke();
        }
    }

    private void TriggerCameraSwitch()
    {
        // This priority swap kicks off the blend
        vcamSpline.Priority = 0;
        vcamPlayer.Priority = 10;
    }
}
