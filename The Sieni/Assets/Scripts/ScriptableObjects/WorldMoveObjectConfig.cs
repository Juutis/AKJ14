using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMoveObjectConfig", menuName = "Configs/New WorldMoveObjectConfig")]
public class WorldMoveObjectConfig : ScriptableObject
{
    [SerializeField]
    private MoveObjectType objectType;
    public MoveObjectType ObjectType { get { return objectType; } }

}
/*
[System.Serializable]
public class MoveObject {
    
}
*/
public enum MoveObjectType
{
    None,
    RegularShroom,
    VisionShroom,
    MoveShroom,
    Tree
}
