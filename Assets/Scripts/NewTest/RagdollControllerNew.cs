using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class RagdollControllerNew : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject rider;
    [SerializeField] Transform riderTransform;
    [SerializeField] CinemachineVirtualCamera ragCamera;
    [SerializeField] CinemachineVirtualCamera playerCamera;

    [Header("Values")]
    [SerializeField] private float setRagdollForce = 5f;
    [SerializeField] private float ragdollDuration = 2f;
    [SerializeField] private float initFOV_ragdoll = 42f;
    [SerializeField] private float finalFOV_ragdoll = 10f;
    [SerializeField] private float lerpSpeed = 2f;

    private float t = 0f;
    private float ragdollForce;
    private Animator riderAnimator;
    private Collider bikeCollider;

    private bool ragdollEnabled = false;
    private bool startZoom = false;

    private void OnEnable()
    {
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void Start()
    {
        ragdollEnabled = false;
        ragdollForce = 0f;

        ragCamera.m_Lens.FieldOfView = initFOV_ragdoll;

        riderTransform = rider.GetComponent<Transform>();
        riderAnimator = rider.GetComponent<Animator>();

        var bikeParent = GameObject.Find("BikeParent");
        bikeCollider = bikeParent != null ? bikeParent.GetComponent<Collider>() : null;

        // Ragdoll bones should stay kinematic (purely animated) until a crash triggers
        // EnableRagdoll(true) — this was never initialized before, so they were non-kinematic
        // and physically simulated from the very start of the game.
        foreach (Rigidbody rb in rider.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!ragdollEnabled) return;

        MoveRagdoll();
        ZoomIntoPlayer();
    }

    private void CollisionHandlerNew_OnPlayerCollision()
    {
        ragdollForce = setRagdollForce;
        ragdollEnabled = true;

        StartCoroutine(DisableMovement());
    }


    private IEnumerator DisableMovement()
    {
        yield return new WaitForSeconds(ragdollDuration);
        ragdollForce = 0;
    }

    public void EnableRagdoll(bool enabled)
    {
        riderAnimator.enabled = !enabled;

        // Scoped to the rider's own bones — this used to scan the whole Character hierarchy,
        // which also toggled the player's own root Rigidbody (breaking normal movement).
        foreach (Rigidbody rb in rider.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !enabled;
        }

        // The rider's bones normally ignore collision with the bike (PlayerController sets
        // this up so the bike doesn't self-collide with its own rider during gameplay). Once
        // ragdoll actually activates, restore that collision so the body realistically reacts
        // to hitting the bike instead of passing through it.
        if (bikeCollider != null)
        {
            foreach (Collider riderCollider in rider.GetComponentsInChildren<Collider>(true))
            {
                Physics.IgnoreCollision(bikeCollider, riderCollider, !enabled);
            }
        }

        ragCamera.Priority = 20;
        playerCamera.Priority = 0;

        startZoom = true;
        t = 0f;
    }

    private void ZoomIntoPlayer()
    {
        if (startZoom)
        {
            t += Time.deltaTime * lerpSpeed;
            ragCamera.m_Lens.FieldOfView = Mathf.Lerp(initFOV_ragdoll, finalFOV_ragdoll, t);

            if (t >= 1f)
            {
                startZoom = false;
            }
        }
    }

    private void MoveRagdoll()
    {
        Vector3 movementForce = new Vector3(
            0, 0, ragdollForce * Time.deltaTime
            );

        riderTransform.Translate(movementForce, Space.World);
    }
}
