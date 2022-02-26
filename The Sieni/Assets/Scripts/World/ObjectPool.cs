using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : MonoBehaviour
{
    //private List<WorldMoveObject> sleepingObjects = new List<WorldMoveObject>();
    //private List<WorldMoveObject> activeObjects = new List<WorldMoveObject>();
    [SerializeField]
    private List<WorldMoveObject> prefabs = new List<WorldMoveObject>();
    [SerializeField]
    private int poolSize = 20;
    [SerializeField]
    private List<MoveObjectType> objectTypes = new List<MoveObjectType>{
        MoveObjectType.MoveShroom,
        MoveObjectType.VisionShroom,
        MoveObjectType.RegularShroom,
        MoveObjectType.Tree,
    };

    [SerializeField]
    private Transform containerPrefab;
    private Dictionary<MoveObjectType, Transform> objectContainers = new Dictionary<MoveObjectType, Transform>();

    private Dictionary<MoveObjectType, List<WorldMoveObject>> activeObjects = new Dictionary<MoveObjectType, List<WorldMoveObject>>();
    private Dictionary<MoveObjectType, List<WorldMoveObject>> sleepingObjects = new Dictionary<MoveObjectType, List<WorldMoveObject>>();

    public void Initialize()
    {
        foreach (MoveObjectType objectType in objectTypes)
        {
            if (!objectContainers.ContainsKey(objectType))
            {
                if (containerPrefab == null)
                {
                    Debug.LogWarning("ObjectPool.containerPrefab is not assigned!");
                    return;
                }
                Transform container = Instantiate(containerPrefab);
                container.SetParent(transform);
                container.position = Vector3.zero;
                container.name = $"Container[{objectType}]";
                objectContainers.Add(objectType, container);
            }
            if (!activeObjects.ContainsKey(objectType))
            {
                activeObjects.Add(objectType, new List<WorldMoveObject>());
            }
            if (!sleepingObjects.ContainsKey(objectType))
            {
                sleepingObjects.Add(objectType, new List<WorldMoveObject>());
            }
            for (int poolIndex = 0; poolIndex < poolSize; poolIndex += 1)
            {
                Add(objectType);
            }
        }
    }

    private WorldMoveObject CreateObject(MoveObjectType objectType)
    {
        WorldMoveObject prefab = prefabs.FirstOrDefault(prefab => prefab.ObjectType == objectType);
        if (prefab == null)
        {
            Debug.LogWarning($"Couldn't create object of type ${objectType} because ObjectPool.prefabs doesn't have one!");
        }
        WorldMoveObject createdObject = Instantiate(prefab);
        createdObject.Initialize($"{objectType}");
        createdObject.Sleep();
        createdObject.transform.parent = objectContainers[objectType];
        return createdObject;
    }

    public void Add(MoveObjectType objectType)
    {
        WorldMoveObject newObject = CreateObject(objectType);
        if (newObject != null)
        {
            sleepingObjects[objectType].Add(newObject);
            int count = sleepingObjects[objectType].Count;
            if (activeObjects.ContainsKey(objectType))
            {
                count += activeObjects[objectType].Count;
            }
            objectContainers[objectType].name = $"Container[{objectType} ({count})]";
        }
    }

    public void Sleep(WorldMoveObject sleepObject)
    {
        sleepObject.Sleep();
        activeObjects[sleepObject.ObjectType].Remove(sleepObject);
        sleepingObjects[sleepObject.ObjectType].Add(sleepObject);
    }

    public WorldMoveObject Get(MoveObjectType objectType)
    {
        if (sleepingObjects[objectType].Count < 1)
        {
            Add(objectType);
        }
        WorldMoveObject wakingUpObject = sleepingObjects[objectType][0];
        sleepingObjects[objectType].Remove(wakingUpObject);
        activeObjects[objectType].Add(wakingUpObject);
        return wakingUpObject;
    }
}
