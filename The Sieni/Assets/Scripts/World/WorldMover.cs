using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    [SerializeField]
    private WorldMoverConfig moveConfig;
    [SerializeField]
    private Transform worldContainer;
    [SerializeField]
    private bool isMoving = true;
    [SerializeField]
    private bool isSpawning = true;

    private List<WorldMoveObject> moveObjects = new List<WorldMoveObject>();

    private Bounds bounds;

    [SerializeField]
    private WorldMoveObject bg;

    private List<Vector2> spawnPositions = new List<Vector2>();

    private float distanceMoved = 0f;

    private int currentStep = 0;

    void Start()
    {
        foreach (MoveObjectSpawn spawn in moveConfig.Spawns)
        {
            spawn.PreviousSpawnStep = 0;
        }
        DetermineBounds();
    }

    private void SpawnRandomObject()
    {
        WorldMoveObject bgObject = Instantiate(bg);
        Register(bgObject);
    }

    private void DetermineBounds()
    {
        bounds = new Bounds(new Vector3(Screen.width / 2, Screen.height / 2, 0f), new Vector3(Screen.width, Screen.height, 0f));
        spawnPositions = new List<Vector2>();
        for (float yPos = WorldBoundsMinY(); yPos < WorldBoundsMaxY(); yPos += 1)
        {
            spawnPositions.Add(new Vector2(SpawnX(), yPos + 0.5f));
        }
        foreach (Vector2 spawnPos in spawnPositions)
        {
            WorldMoveObject spawnInd = Instantiate(bg);
            spawnInd.transform.position = spawnPos;
        }
    }

    private float BoundsMaxX()
    {
        return bounds.max.x;
    }
    private float WorldBoundsMinY()
    {
        return Camera.main.ScreenToWorldPoint(bounds.min).y;
    }
    private float WorldBoundsMaxY()
    {
        return Camera.main.ScreenToWorldPoint(bounds.max).y;
    }

    private float KillZoneX()
    {
        return Camera.main.ScreenToWorldPoint(bounds.min).x - moveConfig.BufferZoneSize;
    }

    private float SpawnX()
    {
        return Camera.main.ScreenToWorldPoint(bounds.max).x + moveConfig.BufferZoneSize;
    }

    private float RandomY()
    {
        return Random.Range(bounds.min.y, bounds.max.y);
    }

    public void Register(WorldMoveObject moveObject)
    {
        moveObjects.Add(moveObject);
    }

    public void Kill(WorldMoveObject moveObject)
    {
        moveObjects.Remove(moveObject);
    }

    void Update()
    {
        MoveObjects();
        SpawnObjects();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomObject();
        }
    }

    private void MoveObjects()
    {
        if (isMoving)
        {
            float step = moveConfig.Speed * Time.deltaTime;
            for (int objectIndex = 0; objectIndex < moveObjects.Count; objectIndex += 1)
            {
                MoveObject(moveObjects[objectIndex], step);
                CheckObject(moveObjects[objectIndex]);
            }
            distanceMoved += step;
            if (distanceMoved >= currentStep)
            {
                currentStep += 1;
                isSpawning = true;
            }
        }
    }

    private void SpawnObjects()
    {
        if (isSpawning)
        {
            List<Vector2> possibleSpawnPoints = new List<Vector2>(spawnPositions);
            foreach (MoveObjectSpawn spawn in moveConfig.Spawns)
            {
                if (Mathf.Abs(spawn.PreviousSpawnStep - currentStep) > spawn.SpawnEveryDistance)
                {
                    spawn.PreviousSpawnStep = currentStep;
                    for (int spawnIndex = 0; spawnIndex < spawn.SpawnAmount; spawnIndex += 1)
                    {
                        Vector2 spawnPosition = possibleSpawnPoints[Random.Range(0, possibleSpawnPoints.Count - 1)];
                        possibleSpawnPoints.Remove(spawnPosition);
                        SpawnObject(spawnPosition, spawn.MoveObject);
                    }
                }
            }
            isSpawning = false;
        }
    }

    private void SpawnObject(Vector2 spawnPosition, WorldMoveObject spawnObjectPrefab)
    {
        WorldMoveObject spawnedObject = Instantiate(spawnObjectPrefab);
        spawnedObject.transform.position = spawnPosition;
        Register(spawnedObject);
    }

    private void MoveObject(WorldMoveObject moveObject, float step)
    {
        Vector2 newPos = moveObject.transform.position;
        newPos.x -= step;
        moveObject.transform.position = newPos;
    }

    private void CheckObject(WorldMoveObject moveObject)
    {
        if (moveObject.transform.position.x < KillZoneX())
        {
            Kill(moveObject);
        }
    }
}
