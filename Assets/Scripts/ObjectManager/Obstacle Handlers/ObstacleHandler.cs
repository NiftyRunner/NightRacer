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

    private bool spawnEnabled = false;

    private void OnEnable()
    {
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    void Start()
    {
        spawnEnabled = true;
        StartCoroutine(SpawnLoop());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Left") || other.gameObject.CompareTag("Right")) {
            objects.ReturnToPool(other.gameObject);
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (spawnEnabled)
        {
            float nextInterval = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(nextInterval);
            if (!spawnEnabled) yield return null;

            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        if (!spawnEnabled) return;

        GameObject currObj = objects.GetCarFromPool();
        if (currObj == null) return;

        ObjectMover curObjMover = currObj.GetComponent<ObjectMover>();

        int rNumber = Random.Range(0, spawnPoints.Count);

        SetSpawnSpeed(curObjMover, rNumber);

        currObj.transform.position = spawnPoints[rNumber].transform.position;
    }

    private void SetSpawnSpeed(ObjectMover curObjMover, int rNumber)
    {
        //Debug.Log("SetSpawnSpeedCalled");
        if (spawnPoints[rNumber].CompareTag("Left"))
        {
            curObjMover.transform.localRotation = spawnPoints[rNumber].localRotation;
            curObjMover.SetAutoForce(leftMoveSpeed);
            curObjMover.gameObject.tag = "Left";
        }
        else if (spawnPoints[rNumber].CompareTag("Right"))
        {
            curObjMover.transform.localRotation = spawnPoints[rNumber].localRotation;
            curObjMover.SetAutoForce(rightMoveSpeed);
            curObjMover.gameObject.tag = "Right";
        }
    }

    private void CollisionHandlerNew_OnPlayerCollision()
    {
        spawnEnabled = false;
    }
}
