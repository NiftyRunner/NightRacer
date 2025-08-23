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

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !enabled;
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
