using Cinemachine;
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

    private float ragdollForce;
    private Animator riderAnimator;

    private bool ragdollEnabled = false;

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
        riderTransform = rider.GetComponent<Transform>();
        riderAnimator = rider.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!ragdollEnabled) return;

        MoveRagdoll();
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
    }

    private void MoveRagdoll()
    {
        Vector3 movementForce = new Vector3(
            0, 0, ragdollForce * Time.deltaTime
            );

        riderTransform.Translate(movementForce, Space.World);
    }
}
