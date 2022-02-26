using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoverDebugger : MonoBehaviour
{
    [SerializeField]
    private WorldMover worldMover;


    private WorldMoverConfig moveConfig;
    private WorldBounds worldBounds;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (worldMover == null)
        {
            return;
        }
        if (moveConfig == null)
        {
            moveConfig = worldMover.MoveConfig;
        }
        if (worldBounds == null)
        {
            worldBounds = worldMover.WorldBounds;
        }
        if (Application.isPlaying || moveConfig == null || !moveConfig.DebuggingEnabled || moveConfig.DebugWasDrawn)
        {
            return;
        }
        worldBounds.DetermineBounds();
        List<Vector2> spawnPositions = worldMover.InitializeSpawnPositions();
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
                List<Vector2> spawnPoints = worldMover.ClaimSpawnPoints(possiblePoints, step + moveConfig.DebugStepStart, spawn, moveConfig.DebugStepStart);
                foreach (Vector2 spawnPoint in spawnPoints)
                {
                    if (spawn.MoveObjectType == MoveObjectType.Tree)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(new Vector2(-moveConfig.DebugStepAmount + spawnPoint.x + step, spawnPoint.y), Vector3.one);
                    }
                    else
                    {

                        Gizmos.color = spawn.MoveObjectType == MoveObjectType.VisionShroom ? Color.magenta : (spawn.MoveObjectType == MoveObjectType.MoveShroom ? Color.cyan : Color.yellow);
                        Gizmos.DrawSphere(new Vector2(-moveConfig.DebugStepAmount + spawnPoint.x + step, spawnPoint.y), 0.5f);
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

        moveConfig.DebugWasDrawn = true;

#endif
    }
}
