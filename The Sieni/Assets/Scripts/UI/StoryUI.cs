using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StoryUI : MonoBehaviour
{
    [SerializeField]
    private Text MoveShroomText;

    [SerializeField]
    private Text VisionShroomText;

    [SerializeField]
    private Text DisableInputsShroomText;


    [SerializeField]
    private Image MoveShroomCanvas;
    [SerializeField]
    private Image VisionShroomCanvas;
    [SerializeField]
    private Image DisableInputShroomCanvas;

    [SerializeField]
    private Color RequirementFullfilledColor;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.main.StoryMode) {
            gameObject.SetActive(false);
        }
         UpdateTexts();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.main.StoryMode) {
            gameObject.SetActive(false);
        }
         UpdateTexts();
    }

    public void UpdateTexts() {
        UpdateText(MoveShroomText, MoveObjectType.MoveShroom, MoveShroomCanvas);
        UpdateText(VisionShroomText, MoveObjectType.VisionShroom, VisionShroomCanvas);
        UpdateText(DisableInputsShroomText, MoveObjectType.DisableControlShroom, DisableInputShroomCanvas);
    }

    private void UpdateText(Text text, MoveObjectType type, Image canvas) {
        var count = GameManager.main.GetCollectedCount(type);
        var required = GameManager.main.shroomsRequiredForWin.Find(it => it.Type == type).Count;
        var textValue = count + "/" + required;
        text.text = textValue;

        if (count >= required) {
            canvas.color = RequirementFullfilledColor;
        }
    }
}
