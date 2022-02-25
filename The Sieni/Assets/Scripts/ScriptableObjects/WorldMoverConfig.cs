using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMoverConfig", menuName = "Configs/New WorldMoverConfig")]
public class WorldMoverConfig : ScriptableObject
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float speedMax;

    [SerializeField]
    private float bufferZoneSize = 1;

    [SerializeField]
    private List<MoveObjectSpawn> spawns = new List<MoveObjectSpawn>();
    public float Speed { get { return speed; } }
    public float SpeedMax { get { return speedMax; } }
    public float BufferZoneSize { get { return bufferZoneSize; } }
    public List<MoveObjectSpawn> Spawns
    {
        get { return spawns; }
    }
}

[System.Serializable]
public class MoveObjectSpawn
{
    [SerializeField]
    private WorldMoveObject moveObject;
    public WorldMoveObject MoveObject { get { return moveObject; } }

    [SerializeField]
    private int spawnEveryDistance = 1;
    public int SpawnEveryDistance { get { return spawnEveryDistance; } }

    [SerializeField]
    private int spawnAmount = 1;

    public int SpawnAmount { get { return spawnAmount; } }

    private int previousSpawnStep = 0;
    public int PreviousSpawnStep
    {
        get { return previousSpawnStep; }
        set
        { previousSpawnStep = value; }
    }
}