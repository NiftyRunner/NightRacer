using System;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private EnvironmentManager envManager;
    [SerializeField] private Transform[] wheels;

    [SerializeField] private float movementMultiplier = 1f;
    [SerializeField] private float autoForce = 1f;
    [SerializeField] private float maxSpinSpeed = 360f;

    private float movementValues;

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
        Debug.Log(movementValues);
    }
    
    void Update()
    {
        MovePlayer();
        RotateWheels();
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

    private void DollyDrive_OnDollyFinished()
    {
        autoForce = 0;
    }
}
