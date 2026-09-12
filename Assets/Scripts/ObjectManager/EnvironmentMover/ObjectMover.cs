using System;
using System.Collections;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private float setAutoForce = 10f;

    [Header("Wheel Settings")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float wheelSpeed = 360f;

    [SerializeField] private float autoForce;

    private bool isObstacle = false;
    private static bool movementEnabled = false;

    private Vector3[] wheelPivotLocalOffsets;

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
        playerController = FindFirstObjectByType<PlayerController>();
        CacheWheelPivotOffsets();

        if(this.gameObject.CompareTag("Left") || this.gameObject.CompareTag("Right"))
        {
            isObstacle = true;
            return;
        }

        //movementEnabled = false;
        autoForce = setAutoForce;
    }

    private void CacheWheelPivotOffsets()
    {
        if (wheels == null) return;

        wheelPivotLocalOffsets = new Vector3[wheels.Length];
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i] == null) continue;

            var renderers = wheels[i].GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) continue;

            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);

            wheelPivotLocalOffsets[i] = wheels[i].InverseTransformPoint(bounds.center);
        }
    }

    void Update()
    {
        if(!movementEnabled) return;

        if (isObstacle && !(autoForce == 0)) { RotateWheels(); }

        MoveObject();   
    }

    private void MoveObject()
    {
        Vector3 movementValues = new Vector3(0, 0, -(autoForce * playerController.GetPlayerSpeedMultiplier())  * Time.deltaTime);
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
        for (int i = 0; i < wheels.Length; i++)
        {
            var w = wheels[i];
            if (w == null) continue;

            Vector3 pivotWorld = (wheelPivotLocalOffsets != null && i < wheelPivotLocalOffsets.Length)
                ? w.TransformPoint(wheelPivotLocalOffsets[i])
                : w.position;

            w.RotateAround(pivotWorld, w.right, deltaAngle);
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
