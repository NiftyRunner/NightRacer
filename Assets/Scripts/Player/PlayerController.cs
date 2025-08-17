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
    [SerializeField] private Animator riderAnimator;
    [SerializeField] float maxTilt = 1f;
    [SerializeField] private float tiltSpeed = 120f;

    private float movementValues;
    private float currentTilt;

    private bool inputEnabled = false;

    private void OnEnable()
    {
        PlayerControlHandler.OnLRValueChange += PlayerControlHandler_OnLRValueChange;
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        PlayerControlHandler.OnLRValueChange -= PlayerControlHandler_OnLRValueChange;
        DollyDrive.OnDollyFinished -= DollyDrive_OnDollyFinished;
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void Start()
    {
        inputEnabled = true;
    }

    private void PlayerControlHandler_OnLRValueChange(float inputValues)
    {
        movementValues = inputValues;
    }
    
    void Update()
    {
        RotateWheels();
        MovePlayer();
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
        if(!inputEnabled) movementValues = 0;

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
        riderAnimator.SetFloat("Tilt", currentTilt);
    }


    private void CollisionHandlerNew_OnPlayerCollision()
    {
        inputEnabled = false;
    }

    private void DollyDrive_OnDollyFinished()
    {
        autoForce = 0;
    }
}
