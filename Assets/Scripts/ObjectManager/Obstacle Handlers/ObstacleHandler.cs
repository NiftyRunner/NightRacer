using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{
    [SerializeField] private ObjectPool objects;
    [SerializeField] List<Transform> spawnPoints;


    [Header("SpawnTimings")]
    [SerializeField] float minSpawnTime = 0f;
    [SerializeField] float maxSpawnTime = 2f;

    [Header("SpawnSpeeds")]
    [SerializeField] float leftMoveSpeed = 5f;
    [SerializeField] float rightMoveSpeed = 15f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle") {
            objects.ReturnToPool(other.gameObject);
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float nextInterval = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(nextInterval);

            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        GameObject currObj = objects.GetCarFromPool();
        if (currObj == null) return;

        ObjectMover curObjMover = currObj.GetComponent<ObjectMover>();

        int rNumber = Random.Range(0, spawnPoints.Count);

        SetSpawnSpeed(curObjMover, rNumber);

        currObj.transform.position = spawnPoints[rNumber].transform.position;
    }

    private void SetSpawnSpeed(ObjectMover curObjMover, int rNumber)
    {
        Debug.Log("SetSpawnSpeedCalled");
        if (spawnPoints[rNumber].CompareTag("Left"))
        {
            curObjMover.transform.localRotation = spawnPoints[rNumber].localRotation;
            curObjMover.SetAutoForce(leftMoveSpeed);
        }
        else if (spawnPoints[rNumber].CompareTag("Right"))
        {
            curObjMover.transform.localRotation = spawnPoints[rNumber].localRotation;
            curObjMover.SetAutoForce(rightMoveSpeed);
        }
    }
}
