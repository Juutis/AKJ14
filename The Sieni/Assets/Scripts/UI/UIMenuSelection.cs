using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuSelection : MonoBehaviour
{
    [SerializeField]
    private Image imgSelector;
    [SerializeField]
    private Text txtTitle;

    [SerializeField]
    private UISelectionType selectionType;

    public UISelectionType SelectionType { get { return selectionType; } }


    private bool isSelected = false;

    public bool IsSelected { get { return isSelected; } }

    private void Start()
    {
        txtTitle.text = selectionType == UISelectionType.Exit ? "Exit" : (
            selectionType == UISelectionType.Endless ? "Endless mode" : (
                selectionType == UISelectionType.Story ? "Story mode" : (
                    selectionType == UISelectionType.MainMenu ? "Main menu" : "Restart"
                )
            )
        );
    }


    public void SetSelected(bool selected)
    {
        isSelected = selected;
        imgSelector.enabled = selected;
    }
}

