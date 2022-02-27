using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIShroomPopup : MonoBehaviour
{
    [SerializeField]
    private Transform buttonContainer;

    [SerializeField]
    private List<UIMoveButtonInfo> moveButtons;
    bool popupping = false;
    public void InitializeButtons(List<UIMoveButtonData> buttonDatas)
    {
        //moveButtons = buttonContainer.transform.GetComponentsInChildren<UIMoveButtonInfo>().ToList();
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
        popupping = true;
        foreach (UIMoveButtonInfo moveButton in moveButtons)
        {
            moveButton.Unhighlight();
        }
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
        popupping = false;
    }

    public void HighlightTraditional()
    {
        if (popupping)
        {
            return;
        }
        float horiz = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");

        float min = 0.1f;
        if (horiz > min)
        {
            moveButtons[3].Highlight();
        }
        else
        {
            moveButtons[3].Unhighlight();
        }
        if (horiz < -min)
        {
            moveButtons[1].Highlight();
        }
        else
        {
            moveButtons[1].Unhighlight();
        }
        if (verti > min)
        {
            moveButtons[0].Highlight();
        }
        else
        {
            moveButtons[0].Unhighlight();
        }
        if (verti < -min)
        {
            moveButtons[2].Highlight();
        }
        else
        {
            moveButtons[2].Unhighlight();
        }
    }

    public void HighlightMoveButtons(Vector2 input)
    {
        if (popupping)
        {
            return;
        }
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

        Dictionary<Direction, InputDirection> mappings = RemappableInput.Main.GetInputMappings();
        foreach (var kvp in mappings)
        {
            Direction key = kvp.Key;
            Direction direction = kvp.Value.Direction;
            if (key == Direction.UP)
            {
                UIMoveButtonInfo moveButton = moveButtons[0];
                moveButton.SetKey(inputsToKeys[direction], inputsToKeyIcons[direction]);
            }
            if (key == Direction.LEFT)
            {
                UIMoveButtonInfo moveButton = moveButtons[1];
                moveButton.SetKey(inputsToKeys[direction], inputsToKeyIcons[direction]);

            }
            if (key == Direction.RIGHT)
            {
                UIMoveButtonInfo moveButton = moveButtons[3];
                moveButton.SetKey(inputsToKeys[direction], inputsToKeyIcons[direction]);

            }
            if (key == Direction.DOWN)
            {
                UIMoveButtonInfo moveButton = moveButtons[2];
                moveButton.SetKey(inputsToKeys[direction], inputsToKeyIcons[direction]);

            }
            //Debug.Log($"Set physical {key} to {inputsToKeyIcons[direction]}");
        }
    }
}
