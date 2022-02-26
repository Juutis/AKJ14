using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoveButtonInfo : MonoBehaviour
{
    [SerializeField]
    private Text txtKey;
    [SerializeField]
    private Text txtIcon;
    [SerializeField]
    private Image imgIcon;
    [SerializeField]
    private Image imgButton;

    private MoveButtonType buttonType;
    public MoveButtonType ButtonType { get { return buttonType; } }

    [SerializeField]
    private Color highlightColor;
    private Color originalColor;


    public void Initialize(UIMoveButtonData buttonData)
    {
        txtKey.text = buttonData.Key.ToString();
        txtIcon.text = buttonData.TextIcon;
        imgIcon.sprite = buttonData.Sprite;
        buttonType = buttonData.ButtonType;
        originalColor = imgButton.color;
    }

    public void SetKey(KeyCode newKey, string keyIcon)
    {
        txtKey.text = newKey.ToString();
        txtIcon.text = keyIcon;
        Debug.Log($"I ({buttonType}) was set to {txtIcon.text}");
    }

    public void Highlight()
    {
        imgButton.color = highlightColor;
    }

    public void Unhighlight()
    {
        imgButton.color = originalColor;
    }
}
