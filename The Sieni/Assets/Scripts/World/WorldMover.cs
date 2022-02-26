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
    private bool isSpawning = false;
    private bool isMovingObjects = false;

    [SerializeField]
    private ObjectPool objectPool;

    private List<WorldMoveObject> moveObjects = new List<WorldMoveObject>();

    private Bounds bounds;
    /*
        [SerializeField]
        private WorldMoveObject bg;*/

    private List<Vector2> spawnPositions = new List<Vector2>();

    private float distanceMoved = 0f;

    private int currentStep = 0;

    private float speedIncrease = 0f;

    private float moveDistance = 0f;

    private int previousSpeedIncreaseStep = 0;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector2.zero,
                new Vector2(
                    Mathf.Abs(minUnitySize.x - maxUnitySize.x),
                    Mathf.Abs(minUnitySize.y - maxUnitySize.y)
                )
            );
        }
        if (Application.isPlaying || moveConfig == null || !moveConfig.DebuggingEnabled || moveConfig.DebugWasDrawn)
        {
            return;
        }
        DetermineBounds();
        foreach (Vector2 spawnPos in spawnPositions)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(spawnPos, 0.4f);
        }

        for (int step = 0; step < moveConfig.DebugStepAmount; step += 1)
        {
            List<Vector2> possiblePoints = new List<Vector2>(spawnPositions);
            foreach (MoveObjectSpawn spawn in moveConfig.Spawns)
            {
                List<Vector2> spawnPoints = ClaimSpawnPoints(possiblePoints, step + moveConfig.DebugStepStart, spawn, moveConfig.DebugStepStart);
                foreach (Vector2 spawnPoint in spawnPoints)
                {
                    if (spawn.MoveObjectType == MoveObjectType.Shroom)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(new Vector2(-moveConfig.DebugStepAmount + spawnPoint.x + step, spawnPoint.y), 0.5f);
                    }
                    else if (spawn.MoveObjectType == MoveObjectType.Tree)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(new Vector2(-moveConfig.DebugStepAmount + spawnPoint.x + step, spawnPoint.y), Vector3.one);
                    }
                }
            }
            if (moveConfig.DebugDrawEmpty)
            {
                foreach (Vector2 spawnPoint in possiblePoints)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawCube(new Vector2(-moveConfig.DebugStepAmount + spawnPoint.x + step, spawnPoint.y), Vector3.one * 0.9f);
                }
            }

        }


        //moveConfig.DebugWasDrawn = true;

#endif
    }

    void Start()
    {
        foreach (MoveObjectSpawn spawn in moveConfig.Spawns)
        {
            spawn.PreviousSpawnStep = 0;
        }
        DetermineBounds();
        objectPool.Initialize();
    }

    void Update()
    {
        CalculateStep();
        MoveObjects();
        SpawnObjects();
        IncreaseSpeed();
    }

    private void CalculateStep()
    {
        if (isMoving)
        {
            moveDistance = (speedIncrease + moveConfig.Speed) * Time.deltaTime;
            distanceMoved += moveDistance;
            if (distanceMoved >= currentStep)
            {
                currentStep += 1;
                isSpawning = true;
                isMovingObjects = true;
            }
        }
    }


    private void IncreaseSpeed()
    {
        if ((speedIncrease + moveConfig.Speed) >= moveConfig.SpeedMax)
        {
            return;
        }
        if (currentStep != 0 && previousSpeedIncreaseStep != currentStep && currentStep % moveConfig.SpeedIncreaseStepInterval == 0)
        {
            previousSpeedIncreaseStep = currentStep;
            speedIncrease += moveConfig.SpeedIncrease;
        }
    }

    private void MoveObjects()
    {
        if (isMovingObjects)
        {
            for (int objectIndex = 0; objectIndex < moveObjects.Count; objectIndex += 1)
            {
                MoveObject(moveObjects[objectIndex], moveDistance);
                CheckObject(moveObjects[objectIndex]);
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
                if (spawn.MoveObjectType == MoveObjectType.None)
                {
                    Debug.LogWarning($"Spawn object type is None!");
                    continue;
                }
                List<Vector2> claimedPoints = ClaimSpawnPoints(possibleSpawnPoints, currentStep, spawn);
                foreach (Vector2 spawnPoint in claimedPoints)
                {
                    SpawnObject(spawnPoint, spawn.MoveObjectType);
                }
            }
            isSpawning = false;
        }
    }

    public void Register(WorldMoveObject moveObject)
    {
        moveObjects.Add(moveObject);
        moveObject.Wakeup();
    }

    public void Sleep(WorldMoveObject moveObject)
    {
        objectPool.Sleep(moveObject);
        moveObjects.Remove(moveObject);
    }

    private void DetermineBounds()
    {
        Vector2 screenSize = GameViewHelper.GetSize();
        float width = screenSize.x;
        float height = screenSize.y;

        Vector3 center = new Vector3(width / 2, height / 2, 0f);
        Vector3 size = new Vector3(width, height, 0f);



        bounds = new Bounds(
            center,
            size
        );

        maxUnitySize = Camera.main.ScreenToWorldPoint(bounds.max);
        minUnitySize = Camera.main.ScreenToWorldPoint(bounds.min);
        unitySize = Camera.main.ScreenToWorldPoint(bounds.size);
        unityCenter = Camera.main.ScreenToWorldPoint(bounds.center);

        spawnPositions = new List<Vector2>();
        for (float yPos = WorldBoundsMinY(); yPos < WorldBoundsMaxY(); yPos += 1)
        {
            spawnPositions.Add(new Vector2(SpawnX(), yPos + 0.5f));
        }

    }

    Vector2 unityCenter;
    Vector2 unitySize;
    Vector2 maxUnitySize;
    Vector2 minUnitySize;

    private float WorldBoundsMinY()
    {
        //return bounds.min.y;
        return minUnitySize.y;
    }
    private float WorldBoundsMaxY()
    {
        //return bounds.max.y;
        return maxUnitySize.y;
    }

    private float KillZoneX()
    {
        //return bounds.max.x - moveConfig.BufferZoneSize;
        return minUnitySize.x - moveConfig.BufferZoneSize;
    }

    private float SpawnX()
    {
        //return bounds.max.x + moveConfig.BufferZoneSize;
        return maxUnitySize.x + moveConfig.BufferZoneSize;
    }

    private List<Vector2> ClaimSpawnPoints(List<Vector2> availablePoints, int step, MoveObjectSpawn spawn, int firstStep = 0)
    {
        List<Vector2> newPoints = new List<Vector2>();
        int offsetStep = step - spawn.SpawnOffset;

        if (step != firstStep && offsetStep % spawn.SpawnInterval == 0)
        {
            for (int spawnIndex = 0; spawnIndex < spawn.IncreasedSpawnAmount(offsetStep); spawnIndex += 1)
            {
                if (availablePoints.Count < 1)
                {
                    Debug.Log($"Ran out of spawn positions for step {step}!");
                    break;
                }
                Vector2 spawnPoint = availablePoints[Random.Range(0, availablePoints.Count - 1)];
                availablePoints.Remove(spawnPoint);
                newPoints.Add(spawnPoint);
            }
        }
        return newPoints;
    }
    private void SpawnObject(Vector2 spawnPosition, MoveObjectType spawnObjectType)
    {
        WorldMoveObject spawnedObject = objectPool.Get(spawnObjectType);
        spawnedObject.transform.position = spawnPosition;
        Register(spawnedObject);
    }

    private void MoveObject(WorldMoveObject moveObject, float moveDistance)
    {
        Vector2 newPos = moveObject.transform.position;
        newPos.x -= moveDistance;
        moveObject.transform.position = newPos;
    }

    private void CheckObject(WorldMoveObject moveObject)
    {
        if (moveObject.transform.position.x < KillZoneX())
        {
            Sleep(moveObject);
        }
    }
}
