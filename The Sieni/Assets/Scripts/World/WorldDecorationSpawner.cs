using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDecorationSpawner : MonoBehaviour
{
    public WorldDecorationConfig DecorationConfig;
    [SerializeField]
    private ObjectPool objectPool;
    [SerializeField]
    private WorldMover worldMover;

    private float distanceMoved = 0f;


    private float lastFillAt = 0f;

    private PoissonDiscSampler sampler;

    private PoissonDiscSampler cloudSampler;

    private PoissonDiscSampler birdSampler;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private Transform cloudContainer;

    [SerializeField]
    private Transform birdContainer;

    private List<WorldMoveObject> moveObjects = new List<WorldMoveObject>();

    private Vector2 screenArea;

    private Vector2 offset;
    void Start()
    {
    }

    public void SetArea(Vector2 area)
    {
        screenArea = area;
        offset = new Vector2(-screenArea.x / 2, -screenArea.y / 2);
        Vector2 areaToFill = new Vector2(area.x * 2, area.y);
        // Debug.Log($"Area to fill: {areaToFill.x}");
        Fill(area);
        Fill(area, area.x);
    }

    public void Move(float distance)
    {
        distanceMoved += distance;
        container.position = new Vector2(container.position.x - distance, container.position.y);
        birdContainer.position = new Vector2(birdContainer.position.x - distance, birdContainer.position.y);
        cloudContainer.position = new Vector2(cloudContainer.position.x - distance * 2, cloudContainer.position.y);
        if (distanceMoved > (lastFillAt + screenArea.x))
        {
            Fill(screenArea, screenArea.x);
        }
        for (int index = 0; index < moveObjects.Count; index += 1)
        {
            {
                WorldMoveObject moveObject = moveObjects[index];
                if (worldMover.CheckObject(moveObject))
                {
                    moveObjects.Remove(moveObject);
                    moveObject.Sleep(objectPool);
                }
            }
        }
    }

    private void Fill(Vector2 area, float xOffset = 0)
    {
        lastFillAt = distanceMoved;
        // Debug.Log($"Filling at {lastFillAt} {xOffset} {offset}");
        sampler = new PoissonDiscSampler(area.x, area.y, DecorationConfig.Radius);
        birdSampler = new PoissonDiscSampler(area.x, area.y, DecorationConfig.Radius * 6);
        cloudSampler = new PoissonDiscSampler(area.x, area.y, DecorationConfig.Radius * 2);

        foreach (Vector2 sampledPos in sampler.Samples())
        {
            WorldMoveObject decorationObject = objectPool.Get(MoveObjectType.Decoration);
            decorationObject.transform.SetParent(container);
            decorationObject.transform.position = new Vector2(xOffset + sampledPos.x + offset.x, sampledPos.y + offset.y);
            decorationObject.Wakeup();
            moveObjects.Add(decorationObject);

        }
        
        foreach (Vector2 sampledPos in cloudSampler.Samples())
        {
            WorldMoveObject cloudObject = objectPool.Get(MoveObjectType.Cloud);
            cloudObject.transform.SetParent(cloudContainer);
            cloudObject.transform.position = new Vector2(xOffset + sampledPos.x + offset.x, sampledPos.y + offset.y);
            cloudObject.Wakeup();
            moveObjects.Add(cloudObject);

        }
        
        foreach (Vector2 sampledPos in birdSampler.Samples())
        {
            WorldMoveObject birdObject = objectPool.Get(MoveObjectType.Bird);
            birdObject.transform.SetParent(birdContainer);
            birdObject.transform.position = new Vector2(xOffset + sampledPos.x + offset.x, sampledPos.y + offset.y);
            birdObject.Wakeup();
            moveObjects.Add(birdObject);

        }
    }
}


