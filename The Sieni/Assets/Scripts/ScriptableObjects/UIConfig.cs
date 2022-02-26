using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/New UIConfig")]
public class UIConfig : ScriptableObject
{
    public List<UIMoveButtonData> UIMoveButtons = new List<UIMoveButtonData>();
}