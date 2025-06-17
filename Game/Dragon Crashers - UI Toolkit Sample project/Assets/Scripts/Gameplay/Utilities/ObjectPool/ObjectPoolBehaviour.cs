using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolBehaviour : MonoBehaviour
{
    // 对象池数据
    public ObjectPoolData data;

    // 存储池化对象的列表
    private List<GameObject> pooledObjects;

    void Awake()
    {
        // 创建对象池
        CreatePool();
    }

    // 创建对象池的方法
    public void CreatePool()
    {
        pooledObjects = new List<GameObject>();

        // 根据指定数量创建对象并添加到对象池
        for (int i = 0; i < data.amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(data.objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    // 从对象池中获取可用对象的方法
    public GameObject GetPooledObject()
    {
        // 遍历对象池，查找未激活的对象
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // 如果对象池需要扩展，则创建新对象并添加到对象池
        if (data.shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(data.objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}