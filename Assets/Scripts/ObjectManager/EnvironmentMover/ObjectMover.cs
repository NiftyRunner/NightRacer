using System;
using System.Collections;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private float setAutoForce = 10f;

    [Header("Wheel Settings")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float wheelSpeed = 360f;

    [SerializeField]
    private float autoForce;
    private bool isObstacle = false;
    private static bool movementEnabled = false;

    public void SetAutoForce(float force) => autoForce = force;

    private void OnEnable()
    {
        DollyDrive.OnDollyStart += DollyDrive_OnDollyStart;
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        DollyDrive.OnDollyStart -= DollyDrive_OnDollyStart;
        DollyDrive.OnDollyFinished -= DollyDrive_OnDollyFinished;
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void Start()
    {
        if(this.gameObject.CompareTag("Left") || this.gameObject.CompareTag("Right"))
        {
            isObstacle = true;
            return;
        }

        //movementEnabled = false;
        autoForce = setAutoForce;
    }

    void Update()
    {
        if(!movementEnabled) return;

        if (isObstacle && !(autoForce == 0)) { RotateWheels(); }

        MoveObject();   
    }

    private void MoveObject()
    {
        Vector3 movementValues = new Vector3(0, 0, -autoForce * Time.deltaTime);
        transform.Translate(movementValues, Space.World);
    }

    private void CollisionHandlerNew_OnPlayerCollision()
    {
        //autoForce = 0;

        if(!isObstacle) autoForce = 0;

        if (this.gameObject.CompareTag("Left"))
        {
            autoForce *= (-1);
        }

        //StartCoroutine(DisableMovement());

    }


    private IEnumerator DisableMovement()
    {
        yield return new WaitForSeconds(2f);
        autoForce = 0;
        //movementEnabled = false;
    }

    private void RotateWheels()
    {
        if (wheels == null) return;

        //float spinRate = Mathf.Lerp(0f, maxSpinSpeed, spline.MaxSpeed / environmentSpeed);
        float deltaAngle = wheelSpeed * Time.deltaTime;
        foreach (var w in wheels)
        {
            w.Rotate(deltaAngle, 0f, 0f, Space.Self);
        }
    }

    private void DollyDrive_OnDollyStart()
    {
        movementEnabled = false;
    }

    private void DollyDrive_OnDollyFinished()
    {
        movementEnabled = true;
    }
}
