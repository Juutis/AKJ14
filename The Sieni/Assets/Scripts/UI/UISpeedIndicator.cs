using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedIndicator : MonoBehaviour
{
    private void Awake()
    {
        main = this;
    }
    public static UISpeedIndicator main;

    [SerializeField]
    private Text txtValue;
    [SerializeField]
    private Image imgSteps;
    private int currentStep = 0;
    private int stepInterval = 0;
    private float speedMax = 0;

    private Transform container;

    void Start()
    {
        stepInterval = WorldMover.main.MoveConfig.SpeedIncreaseStepInterval;
        speedMax = WorldMover.main.MoveConfig.SpeedMax;
        if (GameManager.main.StoryMode)
        {
            container.gameObject.SetActive(false);
        }
    }

    public void SetSpeed(float speed)
    {
        if (GameManager.main.StoryMode)
        {
            return;
        }
        string formattedSpeed = speed.ToString("#.0");
        if (speed >= speedMax)
        {

            txtValue.text = $"Speed: {formattedSpeed} (MAX)";
            return;
        }
        txtValue.text = $"Speed: {formattedSpeed}";
    }

    public void IncreaseStep()
    {
        if (GameManager.main.StoryMode)
        {
            return;
        }
        currentStep += 1;
        if (currentStep >= stepInterval)
        {
            currentStep = 0;
        }
        imgSteps.fillAmount = ((float)currentStep / (float)stepInterval);
    }
}
