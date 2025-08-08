using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    [SerializeField] List<GameObject> cars = new List<GameObject>();


    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private int rNumber;

    private void Awake()
    {
        FillPool();
    }

    private void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            rNumber = Random.Range(0, cars.Count);

            FillCar(cars[rNumber]);
        }
    }

    private void FillCar(GameObject carNo)
    {
        GameObject car = Instantiate(carNo, transform);
        car.SetActive(false);
        pool.Enqueue(car);
    }

    public GameObject GetCarFromPool()
    {
        if (pool.Count == 0) return null;
        GameObject car = pool.Dequeue();
        car.SetActive(true);

        return car;
    }

    public void ReturnToPool(GameObject car)
    {
        car.SetActive(false);
        pool.Enqueue(car);
    }

}
