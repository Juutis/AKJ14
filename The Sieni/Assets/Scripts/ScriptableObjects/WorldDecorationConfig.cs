using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDecorationConfig", menuName = "Configs/New WorldDecorationConfig")]
public class WorldDecorationConfig : ScriptableObject
{
    [SerializeField]
    [Range(0.1f, 5)]
    private float radius;
    public float Radius { get { return radius; } }
}
