using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "WorldMoverConfig", menuName = "Configs/New WorldMoverConfig")]
public class WorldMoverConfig : ScriptableObject
{
    [Header("General")]
    [SerializeField]
    private float speed;

    [SerializeField]
    [Range(0, 1)]
    private float speedIncrease;
    [SerializeField]
    [Range(1, 1000)]
    private int speedIncreaseStepInterval;
    [SerializeField]
    private float speedMax;

    [SerializeField]
    private float bufferZoneSize = 1;

    [SerializeField]
    private List<MoveObjectSpawn> spawns = new List<MoveObjectSpawn>();

    public float Speed { get { return speed; } }
    public float SpeedIncrease { get { return speedIncrease; } }
    public int SpeedIncreaseStepInterval { get { return speedIncreaseStepInterval; } }
    public float SpeedMax { get { return speedMax; } }
    public float BufferZoneSize { get { return bufferZoneSize; } }
    public List<MoveObjectSpawn> Spawns
    {
        get { return spawns.Where(spawn => spawn.IsEnabled).ToList(); }
    }

    [Header("Debugging")]
    [SerializeField]
    private bool debuggingEnabled = false;
    [SerializeField]
    [Range(20, 1000)]
    private int debugStepAmount = 20;
    [SerializeField]
    [Range(0, 1000)]
    private int debugStepStart = 0;
    [SerializeField]
    private bool debugDrawEmpty = false;
    public bool DebuggingEnabled { get { return debuggingEnabled; } }
    public int DebugStepAmount { get { return debugStepAmount; } }
    public int DebugStepStart { get { return debugStepStart; } }
    public bool DebugDrawEmpty { get { return debugDrawEmpty; } }

    private bool debugWasDrawn = false;

    public bool DebugWasDrawn { set { debugWasDrawn = value; } get { return debugWasDrawn; } }
    public void OnValidate()
    {
        debugWasDrawn = false;
    }
}

[System.Serializable]
public class MoveObjectSpawn
{
    [SerializeField]
    private bool isEnabled = true;

    [SerializeField]
    private MoveObjectType moveObjectType;


    [SerializeField]
    [Range(1, 20)]
    private int spawnInterval = 1;

    [SerializeField]
    [Range(0, 20)]
    private int spawnOffset = 0;

    [SerializeField]
    [Range(1, 8)]
    private int spawnAmount = 1;

    [SerializeField]
    [Range(1, 8)]
    private int maxSpawns = 8;
    [SerializeField]
    [Range(0, 1000)]
    private int increaseStepInterval = 1;
    private int previousSpawnStep = 0;
    public bool IsEnabled { get { return isEnabled; } }
    public int SpawnInterval { get { return spawnInterval; } }
    public int SpawnOffset { get { return spawnOffset; } }

    public MoveObjectType MoveObjectType { get { return moveObjectType; } }
    //public int SpawnAmount { get { return spawnAmount; } }

    public int IncreasedSpawnAmount(int step)
    {
        if (increaseStepInterval == 0)
        {
            return spawnAmount;
        }
        int offsetStep = step - spawnInterval;
        if (offsetStep < 1)
        {
            return spawnAmount;
        }
        int increase = offsetStep / spawnInterval / increaseStepInterval;
        return System.Math.Clamp(spawnAmount + increase, 0, maxSpawns);
    }

    public int MaxSpawns { get { return maxSpawns; } }

    public int IncreaseStepInterval { get { return increaseStepInterval; } }

    public int PreviousSpawnStep
    {
        get { return previousSpawnStep; }
        set
        { previousSpawnStep = value; }
    }
}