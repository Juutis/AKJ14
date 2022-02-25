using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMoveObjectConfig", menuName = "Configs/New WorldMoveObjectConfig")]
public class WorldMoveObjectConfig : ScriptableObject
{
    private float speed;
    public float Speed {get {return speed;}}
    
    
}
