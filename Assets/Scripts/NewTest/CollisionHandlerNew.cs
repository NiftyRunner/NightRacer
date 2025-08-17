using UnityEngine;

public class CollisionHandlerNew : MonoBehaviour
{
    private RagdollControllerNew ragController;

    private void Start()
    {
        ragController = GetComponent<RagdollControllerNew>();
        //ragController.EnableRagdoll(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            ragController.EnableRagdoll(true);
        }
    }
}
