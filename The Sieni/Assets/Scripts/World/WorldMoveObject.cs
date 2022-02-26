using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMoveObject : MonoBehaviour
{
    [SerializeField]
    private WorldMoveObjectConfig moveConfig;

    public MoveObjectType ObjectType { get { return moveConfig.ObjectType; } }

}
