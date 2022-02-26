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

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
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
        /*Gizmos.color = Color.green;

        Gizmos.DrawSphere(Vector3.zero, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.up, Vector3.one);

        Gizmos.color = Color.white;*/
        //Debug.Log("Draw");
        for (int step = 0; step < moveConfig.DebugStepAmount; step += 1)
        {
            List<Vector2> possiblePoints = new List<Vector2>(spawnPositions);
            //Debug.Log($"Step: {step}");
            foreach (MoveObjectSpawn spawn in moveConfig.Spawns)
            {
                List<Vector2> spawnPoints = ClaimSpawnPoints(possiblePoints, step + moveConfig.DebugStepStart, spawn, moveConfig.DebugStepStart);
                foreach (Vector2 spawnPoint in spawnPoints)
                {
                    if (spawn.MoveObject.ObjectType == MoveObjectType.Shroom)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(new Vector2(-moveConfig.DebugStepAmount + spawnPoint.x + step, spawnPoint.y), 0.5f);
                    }
                    else if (spawn.MoveObject.ObjectType == MoveObjectType.Tree)
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
                List<Vector2> claimedPoints = ClaimSpawnPoints(possibleSpawnPoints, currentStep, spawn);
                foreach (Vector2 spawnPoint in claimedPoints)
                {
                    SpawnObject(spawnPoint, spawn.MoveObject);
                }
            }
            isSpawning = false;
        }
    }

    public void Register(WorldMoveObject moveObject)
    {
        moveObjects.Add(moveObject);
    }

    public void Kill(WorldMoveObject moveObject)
    {
        moveObjects.Remove(moveObject);
    }

    private void SpawnRandomObject()
    {
        WorldMoveObject bgObject = Instantiate(bg);
        Register(bgObject);
    }

    private void DetermineBounds()
    {
        Vector2 size = GameViewHelper.GetSize();
        float width = size.x;
        float height = size.y;

        bounds = new Bounds(new Vector3(width / 2, height / 2, 0f), new Vector3(width, height, 0f));
        spawnPositions = new List<Vector2>();
        for (float yPos = WorldBoundsMinY(); yPos < WorldBoundsMaxY(); yPos += 1)
        {
            spawnPositions.Add(new Vector2(SpawnX(), yPos + 0.5f));
        }
        /*foreach (Vector2 spawnPos in spawnPositions)
        {
            WorldMoveObject spawnInd = Instantiate(bg);
            spawnInd.transform.position = spawnPos;
        }*/
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
