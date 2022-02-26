using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBounds : MonoBehaviour
{

    private Vector2 maxUnitySize;
    private Vector2 minUnitySize;

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
#endif
    }

    public void DetermineBounds()
    {
        Vector2 screenSize = GameViewHelper.GetSize();
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

}
