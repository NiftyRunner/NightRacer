using UnityEngine;
using System.Collections.Generic;
using System;

public class EnvironmentManager : MonoBehaviour
{   
    [Header("Pooling")]
    [SerializeField] private List<GameObject> segmentPrefabs;
    [SerializeField] private int initialSegments = 5;

    [Header("Config")]
    [SerializeField] private Transform firstSpawnPoint;

    private Queue<GameObject> segmentsQueue;
    private List<GameObject> activeSegments = new List<GameObject>();

    private void Awake()
    {
        segmentsQueue = new Queue<GameObject>();

        // Pre-instantiate your pool
        foreach (var prefab in segmentPrefabs)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            segmentsQueue.Enqueue(go);
        }
    }

    private void Start()
    {
        Transform spawnPoint = firstSpawnPoint;
        for (int i = 0; i < initialSegments; i++)
        {
            // Spawn i segments at startup
            GameObject seg = SpawnNextAt(spawnPoint);
            NextSegmentSpawner spawner = seg.GetComponent<NextSegmentSpawner>();
            spawnPoint = spawner.GetNextSegSpawnPoint();
        }
    }

    private GameObject SpawnNextAt(Transform spawnPoint)
    {
        if (segmentsQueue.Count == 0)
        {
            Debug.LogWarning("Pool empty!");
            return null;
        }

        GameObject segment = segmentsQueue.Dequeue();
        segment.transform.position = spawnPoint.position;
        segment.transform.rotation = spawnPoint.rotation;
        segment.SetActive(true);

        activeSegments.Add(segment);
        return segment;
    }

    public void AdvanceSegment()
    {
        // 1) Spawn a new segment at the tail:
        GameObject last = activeSegments[activeSegments.Count - 1];
        var spawner = last.GetComponentInChildren<NextSegmentSpawner>();
        GameObject next = SpawnNextAt(spawner.GetNextSegSpawnPoint());

        // 2) Recycle the oldest:
        GameObject old = activeSegments[0];
        activeSegments.RemoveAt(0);
        old.SetActive(false);
        segmentsQueue.Enqueue(old);
    }

}
