using UnityEngine;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Pooling")]
    public ObjectPool GenPool;

    [Header("Config")]
    [SerializeField] Vector3 spawnPos;

    GameObject SpawnFromPool(ObjectPool pool, Vector3 pos)
    {
        GameObject obj = pool.Get();
        obj.transform.position = pos;
        return obj;
    }
}
