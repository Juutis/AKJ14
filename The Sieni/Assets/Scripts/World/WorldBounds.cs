using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBounds : MonoBehaviour
{

    private Vector2 maxUnitySize;
    private Vector2 minUnitySize;

    [SerializeField]
    private WorldMover worldMover;

    public static WorldBounds main;
    private void Awake()
    {
        main = this;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        DetermineBounds();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector2.zero, GetSize());
#endif
    }

    public void DetermineBounds()
    {
        Vector2 screenSize;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            screenSize = GameViewHelper.GetSize();
        }
        else
        {
            screenSize = new Vector2(Screen.width, Screen.height);
        }
#endif
        if (Application.isPlaying)
        {
            screenSize = new Vector2(Screen.width, Screen.height);
        }

        float width = screenSize.x;
        float height = screenSize.y;

        Vector3 center = new Vector3(width / 2, height / 2, 0f);
        Vector3 size = new Vector3(width, height, 0f);

        Bounds bounds = new Bounds(
            center,
            size
        );

        maxUnitySize = Camera.main.ScreenToWorldPoint(bounds.max);
        minUnitySize = Camera.main.ScreenToWorldPoint(bounds.min);
        maxUnitySize.y -= 2f;
        minUnitySize.y += 1f;
    }

    public float WorldBoundsMinY()
    {
        return minUnitySize.y;
    }
    public float WorldBoundsMaxY()
    {
        return maxUnitySize.y;
    }

    public float KillZoneX(float bufferZoneSize)
    {
        return minUnitySize.x - bufferZoneSize;
    }

    public float SpawnX(float bufferZoneSize)
    {
        return maxUnitySize.x + bufferZoneSize;
    }

    public Vector2 GetSize()
    {
        return
            new Vector2(
                Mathf.Abs(minUnitySize.x - maxUnitySize.x),
                Mathf.Abs(minUnitySize.y - maxUnitySize.y)
            );
    }

    private float width = Screen.width;
    private float height = Screen.height;

    void Update()
    {
        if (Screen.width != width || Screen.height != height)
        {
            width = Screen.width;
            height = Screen.height;
            Debug.Log("Bounds changed, determining...");
            DetermineBounds();
            worldMover.RefreshSpawns();
        }
    }

}
