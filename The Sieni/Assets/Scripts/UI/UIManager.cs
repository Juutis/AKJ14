using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager main;
    private void Awake()
    {
        main = this;
    }
    [SerializeField]
    private UIConfig uiConfig;
    [SerializeField]
    private UIShroomPopup uiShroomPopup;


    void Start()
    {
        uiShroomPopup.InitializeButtons(uiConfig.UIMoveButtons);
    }

    public void RemapButtons()
    {
        uiShroomPopup.Popup();
    }

    public void HighlightMoveButtons(Vector2 input)
    {
        //uiShroomPopup.HighlightMoveButtons(input);
        uiShroomPopup.HighlightTraditional();
    }
}

[System.Serializable]
public class UIMoveButtonData
{
    public KeyCode Key;
    public string TextIcon;
    public MoveButtonType ButtonType;
    public Sprite Sprite;
}


public enum MoveButtonType
{
    Top,
    Right,
    Bottom,
    Left
}
