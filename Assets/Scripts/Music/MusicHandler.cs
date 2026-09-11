using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioSource bgSource;
    [SerializeField] AudioClip mainClip;
    [SerializeField] AudioClip endClip;

    private void OnEnable()
    {
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void CollisionHandlerNew_OnPlayerCollision()
    {
        playerSource.Stop();
        bgSource.Stop();
        bgSource.loop = false;
        bgSource.clip = endClip;
        bgSource.Play();
    }

    void Start()
    {
        playerSource.Play();
        bgSource.clip = mainClip;
        bgSource.Play();
    }

}
