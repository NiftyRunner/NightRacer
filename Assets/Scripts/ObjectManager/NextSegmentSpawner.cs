using UnityEngine;

public class NextSegmentSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform nextSegSpawnPoint;  // assign this in the prefab inspector!

    public Transform GetNextSegSpawnPoint() => nextSegSpawnPoint;
}
