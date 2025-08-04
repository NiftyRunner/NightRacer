using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody p_Body;

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
    }

    private void OnDisable()
    {
        PlayerControlHandler.OnLRValueChange -= PlayerControlHandler_OnLRValueChange;
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

    private void MovePlayer()
    {
        Vector3 movementForce = new Vector3(
            movementValues * movementMultiplier * Time.deltaTime, 0, autoForce*Time.deltaTime
            );
        
        transform.Translate(movementForce, Space.World);
        //p_Body.AddForce(movementForce, ForceMode.Force);
    }
}
