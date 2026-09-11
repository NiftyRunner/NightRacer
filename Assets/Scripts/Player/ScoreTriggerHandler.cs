using UnityEngine;

public class ScoreTriggerHandler : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] float nearMissDistance = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Right") || other.gameObject.CompareTag("Left"))
        {
            float distance = Vector3.Distance(transform.position , other.transform.position);
            if (distance < nearMissDistance) {
                ScoreManager.Instance.AddNearMiss(controller.GetPlayerSpeed());
            }
        }
    }
}
