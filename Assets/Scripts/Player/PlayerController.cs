using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody p_Body;

    [SerializeField] private EnvironmentManager envManager;
    [SerializeField] private float movementMultiplier = 1f;
    [SerializeField] private float autoForce = 1f;
    private float movementValues;


    private void Awake()
    {
        p_Body = GetComponent<Rigidbody>();
    }

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
