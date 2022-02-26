using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveObject : MonoBehaviour
{
    [SerializeField]
    private WorldMoveObjectConfig moveConfig;

    public MoveObjectType ObjectType { get { return moveConfig.ObjectType; } }

    private string originalName;

    [SerializeField]
    GameObject container;

    public void Initialize(string newName)
    {
        originalName = newName;
    }

    public void Sleep()
    {
        container.SetActive(false);
        name = $"{originalName} (*sleepy*)";
    }

    public void Wakeup()
    {
        container.SetActive(true);
        name = $"{originalName} [active]";
    }
}
