using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance;
    private readonly Dictionary<string, Queue<GameObject>> _objectPool = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else if(Instance != null) 
            Destroy(gameObject);
    }

    public GameObject GetObjectFromPool(GameObject currentObject)
    {
        if (!_objectPool.TryGetValue(currentObject.name, out var objectList))
            return CreateNewObject(currentObject);
        
        if(objectList.Count == 0)
            return CreateNewObject(currentObject);

        var newObject = objectList.Dequeue();
        newObject.SetActive(true);
        
        return newObject;
    }

    private GameObject CreateNewObject(GameObject gameObjectToCreate)
    {
        var newObject = Instantiate(gameObjectToCreate, transform, true);
        newObject.name = gameObjectToCreate.name;

        return newObject;
    }

    public void ReturnToPool(GameObject currentActiveObject)
    {
        if (_objectPool.TryGetValue(currentActiveObject.name, out var objectList))
        {
            objectList.Enqueue(currentActiveObject);
        }
        else
        {
            var currentObjectQueue = new Queue<GameObject>();
            currentObjectQueue.Enqueue(currentActiveObject);
            _objectPool.Add(currentActiveObject.name, currentObjectQueue);
        }

        currentActiveObject.SetActive(false);
    }
}
