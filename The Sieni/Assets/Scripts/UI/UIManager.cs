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
    [SerializeField]
    private UIScore uiScore;
    [SerializeField]
    private UIGameOver uiGameOver;

    [SerializeField]
    private UIPoppingText popPrefab;
    [SerializeField]
    private Transform popContainer;

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

    public void UpdateScore(int multiplier, int totalScore, Dictionary<MoveObjectType, int> collectedShrooms)
    {
        uiScore.UpdateScore(multiplier, totalScore);
        uiGameOver.UpdateScore(multiplier, totalScore, collectedShrooms);
    }


    public void ShowPoppingMessage(Vector2 pos, int message)
    {
        ShowPoppingMessage(pos, message.ToString());
    }

    public void ShowPoppingMessage(Vector2 pos, string message)
    {
        UIPoppingText popText = Instantiate(popPrefab);
        popText.transform.SetParent(popContainer);
        popText.Show(pos, message);
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
