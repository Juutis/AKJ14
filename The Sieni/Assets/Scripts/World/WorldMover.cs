using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    public static WorldMover main;
    private void Awake()
    {
        main = this;
    }

    [SerializeField]
    private WorldMoverConfig moveConfig;
    public WorldMoverConfig MoveConfig { get { return moveConfig; } }

    [SerializeField]
    private bool isMoving = true;
    private bool isSpawning = false;
    private bool isMovingObjects = false;

    [SerializeField]
    private ObjectPool objectPool;
    [SerializeField]
    private WorldBounds worldBounds;
    public WorldBounds WorldBounds { get { return worldBounds; } }

    [SerializeField]
    private Material scrollingMaterial;

    private List<WorldMoveObject> moveObjects = new List<WorldMoveObject>();

    private List<Vector2> spawnPositions = new List<Vector2>();

    private float distanceMoved = 0f;

    private int currentStep = 0;

    private float speedIncrease = 0f;

    private float moveDistance = 0f;

    private int previousSpeedIncreaseStep = 0;

    private int minEmptyPositionsPerStep = 3;

    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

    [SerializeField]
    private WorldDecorationSpawner decorationSpawner;

    void Start()
    {
        foreach (MoveObjectSpawn spawn in moveConfig.Spawns)
        {
            spawn.PreviousSpawnStep = 0;
        }
        worldBounds.DetermineBounds();
        spawnPositions = InitializeSpawnPositions();
        objectPool.Initialize();
        decorationSpawner.SetArea(worldBounds.GetSize());

#if UNITY_EDITOR
        distanceMoved += moveConfig.PlaytestStepOffset;
        currentStep += moveConfig.PlaytestStepOffset;
        speedIncrease = System.Math.Clamp(
            (currentStep / moveConfig.SpeedIncreaseStepInterval) * moveConfig.SpeedIncrease,
            0,
            moveConfig.SpeedMax - moveConfig.Speed
        );
#endif

        scrollingMaterial.SetFloat("_Offset_X", 0.0f);
    }

    void Update()
    {
        if (!isMoving)
        {
            return;
        }
        CalculateStep();
        MoveObjects();
        SpawnObjects();
        IncreaseSpeed();
        UpdateScrollingMaterialSpeed();
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

    public List<Vector2> InitializeSpawnPositions()
    {
        List<Vector2> points = new List<Vector2>();
        for (float yPos = worldBounds.WorldBoundsMinY(); yPos < worldBounds.WorldBoundsMaxY(); yPos += 1)
        {
            points.Add(new Vector2(worldBounds.SpawnX(moveConfig.BufferZoneSize), yPos + 0.5f));
        }
        return points;

    }

    public List<Vector2> ClaimSpawnPoints(List<Vector2> availablePoints, int step, MoveObjectSpawn spawn, int firstStep = 0)
    {
        List<Vector2> newPoints = new List<Vector2>();
        bool offsetIsPassed = step > spawn.SpawnOffset;
        bool stepIsNotFirst = step != firstStep;
        int offsetStep = step - spawn.SpawnOffset;
        bool stepHitInterval = offsetStep % spawn.SpawnInterval == 0;

        if (stepIsNotFirst && offsetIsPassed && stepHitInterval)
        {
            for (int spawnIndex = 0; spawnIndex < spawn.IncreasedSpawnAmount(offsetStep); spawnIndex += 1)
            {
                if (availablePoints.Count < minEmptyPositionsPerStep)
                {
                    Debug.LogWarning($"Not enough spawn points on step {step} (must have {minEmptyPositionsPerStep} empty ones).");
                    break;
                }
                Vector2 spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
                availablePoints.Remove(spawnPoint);
                newPoints.Add(spawnPoint);
            }
        }
        return newPoints;
    }

    private void CalculateStep()
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

    private void UpdateScrollingMaterialSpeed()
    {
        var offset = scrollingMaterial.GetFloat("_Offset_X");
        scrollingMaterial.SetFloat("_Offset_X", offset + moveDistance / 2.0f);
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
                bool isOut = CheckObject(moveObjects[objectIndex]);
                if (isOut)
                {
                    Sleep(moveObjects[objectIndex]);
                }
            }
            if (decorationSpawner != null)
            {
                decorationSpawner.Move(moveDistance);
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

    private void SpawnObject(Vector2 spawnPosition, MoveObjectType spawnObjectType)
    {
        WorldMoveObject spawnedObject = objectPool.Get(spawnObjectType);
        spawnedObject.transform.position = spawnPosition;
        Register(spawnedObject);
    }

    private void MoveObject(WorldMoveObject moveObject, float moveDistance)
    {
        Vector2 newPos = moveObject.transform.position;
        newPos.x -= moveDistance * (moveObject.ObjectType == MoveObjectType.Cloud ? 20f : 1f);
        if(moveObject.ObjectType == MoveObjectType.Cloud) Debug.Log("moi");
        moveObject.transform.position = newPos;
    }

    public bool CheckObject(WorldMoveObject moveObject)
    {
        return moveObject.transform.position.x < worldBounds.KillZoneX(moveConfig.BufferZoneSize);
    }
}
