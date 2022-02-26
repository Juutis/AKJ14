using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIShroomPopup : MonoBehaviour
{
    [SerializeField]
    private Transform buttonContainer;

    List<UIMoveButtonInfo> moveButtons;
    public void InitializeButtons(List<UIMoveButtonData> buttonDatas)
    {
        moveButtons = buttonContainer.transform.GetComponentsInChildren<UIMoveButtonInfo>().ToList();
        foreach (UIMoveButtonData buttonData in buttonDatas)
        {
            if (buttonData.ButtonType == MoveButtonType.Top)
            {
                moveButtons[0].Initialize(buttonData);
            }
            if (buttonData.ButtonType == MoveButtonType.Left)
            {
                moveButtons[1].Initialize(buttonData);
            }
            if (buttonData.ButtonType == MoveButtonType.Bottom)
            {
                moveButtons[2].Initialize(buttonData);
            }
            if (buttonData.ButtonType == MoveButtonType.Right)
            {
                moveButtons[3].Initialize(buttonData);
            }
        }
    }

    [SerializeField]
    private Animator animator;

    public void Popup()
    {
        Time.timeScale = 0f;
        animator.Play("Show");
    }

    public void ShowFinished()
    {
        RemapUIButtons();
        animator.Play("Hide");
    }

    public void HideFinished()
    {
        Time.timeScale = 1f;
    }

    public void HighlightMoveButtons(Vector2 input)
    {
        float min = 0.1f;
        UIMoveButtonInfo top = GetButton(MoveButtonType.Top);
        UIMoveButtonInfo left = GetButton(MoveButtonType.Left);
        UIMoveButtonInfo right = GetButton(MoveButtonType.Right);
        UIMoveButtonInfo bottom = GetButton(MoveButtonType.Bottom);
        if (input.x > min)
        {
            right.Highlight();
        }
        else
        {
            right.Unhighlight();
        }
        if (input.x < -min)
        {
            left.Highlight();
        }
        else
        {
            left.Unhighlight();
        }
        if (input.y > min)
        {
            top.Highlight();
        }
        else
        {
            top.Unhighlight();
        }
        if (input.y < -min)
        {
            bottom.Highlight();
        }
        else
        {
            bottom.Unhighlight();
        }
    }

    public UIMoveButtonInfo GetButton(MoveButtonType moveButtonType)
    {
        return moveButtons.FirstOrDefault(button => button.ButtonType == moveButtonType);
    }

    Dictionary<Direction, KeyCode> inputsToKeys = new Dictionary<Direction, KeyCode>{
        {Direction.UP, KeyCode.W},
        {Direction.LEFT, KeyCode.A},
        {Direction.RIGHT, KeyCode.D},
        {Direction.DOWN, KeyCode.S}
    };
    Dictionary<Direction, string> inputsToKeyIcons = new Dictionary<Direction, string>{
        {Direction.UP, "↑"},
        {Direction.LEFT, "←"},
        {Direction.RIGHT, "→"},
        {Direction.DOWN, "↓"}
    };
    public void RemapUIButtons()
    {
        Dictionary<Direction, Direction> mappings = RemappableInput.Main.GetInputMappings();
        foreach (KeyValuePair<Direction, Direction> kvp in mappings)
        {
            Direction key = kvp.Key;
            Direction direction = kvp.Value;
            if (direction == Direction.UP)
            {
                UIMoveButtonInfo moveButton = GetButton(MoveButtonType.Top);
                moveButton.SetKey(inputsToKeys[key], inputsToKeyIcons[key]);
            }
            if (direction == Direction.LEFT)
            {
                UIMoveButtonInfo moveButton = GetButton(MoveButtonType.Left);
                moveButton.SetKey(inputsToKeys[key], inputsToKeyIcons[key]);
            }
            if (direction == Direction.RIGHT)
            {
                UIMoveButtonInfo moveButton = GetButton(MoveButtonType.Right);
                moveButton.SetKey(inputsToKeys[key], inputsToKeyIcons[key]);
            }
            if (direction == Direction.DOWN)
            {
                UIMoveButtonInfo moveButton = GetButton(MoveButtonType.Bottom);
                moveButton.SetKey(inputsToKeys[key], inputsToKeyIcons[key]);
            }
        }
    }
}
