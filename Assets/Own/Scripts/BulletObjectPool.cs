using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{
    public static BulletObjectPool Instance;

    [SerializeField]
    private GameObject poolingObjectPrefab;

    private Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        Initialize(30);
    }

    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(poolingObjectPrefab, transform);
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private void Initialize(int count)
    {
        for(int i = 0; i < count; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject());
        }
    }

    public static GameObject GetObject()
    {
        if(Instance.poolingObjectQueue.Count > 0)
        {
            GameObject obj = Instance.poolingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = Instance.CreateNewObject();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
    public static void ReturnObject(GameObject bullet)
    {
        ResetObject(bullet);
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(bullet);
    }
    private static void ResetObject(GameObject bullet)
    {
        bullet.transform.position = Vector3.zero;
        bullet.transform.rotation = Quaternion.identity;
    }
}
