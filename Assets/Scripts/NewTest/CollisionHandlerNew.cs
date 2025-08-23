using System;
using UnityEngine;

public class CollisionHandlerNew : MonoBehaviour
{
    public static event Action OnPlayerCollision;

    [SerializeField] private TweenHandler gameOverTweenHandler;

    private RagdollControllerNew ragController;


    private bool collided = false;
    private void Start()
    {
        collided = false;
        ragController = GetComponent<RagdollControllerNew>();
        //ragController.EnableRagdoll(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Left" || collision.gameObject.tag == "Right") && !collided)
        {
            collided = true;
            gameOverTweenHandler.PlaySequence();
            OnPlayerCollision?.Invoke();
            ragController.EnableRagdoll(true);
        }
    }


}
