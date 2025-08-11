using System;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private EnvironmentManager envManager;
    [SerializeField] private Transform[] wheels;

    [SerializeField] private float movementMultiplier = 1f;
    [SerializeField] private float autoForce = 1f;
    [SerializeField] private float maxSpinSpeed = 360f;

    [Header("Tilt Values")]
    [SerializeField] private Animator bikeAnimator;
    [SerializeField] float maxTilt = 1f;
    [SerializeField] private float tiltSpeed = 120f;

    private float movementValues;
    private float currentTilt;

    private void OnEnable()
    {
        PlayerControlHandler.OnLRValueChange += PlayerControlHandler_OnLRValueChange;
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
    }

    private void OnDisable()
    {
        PlayerControlHandler.OnLRValueChange -= PlayerControlHandler_OnLRValueChange;
        DollyDrive.OnDollyFinished -= DollyDrive_OnDollyFinished;
    }

    private void PlayerControlHandler_OnLRValueChange(float inputValues)
    {
        movementValues = inputValues;
    }
    
    void Update()
    {
        MovePlayer();
        RotateWheels();
        TiltPlayer();
    }

    private void RotateWheels()
    {
        if (wheels == null) return;

        //float spinRate = Mathf.Lerp(0f, maxSpinSpeed, spline.MaxSpeed / environmentSpeed);
        float deltaAngle = maxSpinSpeed * Time.deltaTime;
        foreach (var w in wheels)
        {
            w.Rotate(-deltaAngle, 0f, 0f, Space.Self);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "NextSeg")
        {
            envManager.AdvanceSegment();
        }
    }

    private void MovePlayer()
    {
        Vector3 movementForce = new Vector3(
            movementValues * movementMultiplier * Time.deltaTime, 0, autoForce*Time.deltaTime
            );
        
        transform.Translate(movementForce, Space.World);
        //p_Body.AddForce(movementForce, ForceMode.Force);
    }

    private void TiltPlayer()
    {
        currentTilt = Mathf.Lerp(currentTilt, movementValues, tiltSpeed * Time.deltaTime);

        bikeAnimator.SetFloat("Tilt", currentTilt);

        //float targetX = baseXAngle - movementValues * maxTiltDelta;

        //// Create the target rotation from Euler angles
        //Quaternion targetRot = Quaternion.Euler(targetX, bikeTiltTransform.localEulerAngles.y, bikeTiltTransform.localEulerAngles.z);

        //// Smoothly rotate towards target
        //bikeTiltTransform.localRotation = Quaternion.Slerp(
        //    bikeTiltTransform.localRotation,
        //    targetRot,
        //    tiltSpeed * Time.deltaTime
        //);
    }

    private void DollyDrive_OnDollyFinished()
    {
        autoForce = 0;
    }
}
