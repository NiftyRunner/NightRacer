using Cinemachine;
using UnityEngine;

public class RagdollControllerNew : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] CinemachineVirtualCamera ragCamera;
    [SerializeField] CinemachineVirtualCamera playerCamera;

    public void EnableRagdoll(bool enabled)
    {
        playerAnimator.enabled = !enabled;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !enabled;
        }

        ragCamera.Priority = 20;
        playerCamera.Priority = 0;
    }
}
