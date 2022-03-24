using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuScores : MonoBehaviour
{
    [SerializeField]
    private Text txtStoryScore;
    [SerializeField]
    private Text txtEndlessScore;
    void Start()
    {
        int storyHighScore = PlayerPrefs.GetInt($"highScore_story", 0);
        int endlessHighScore = PlayerPrefs.GetInt($"highScore_endless", 0);
        if (storyHighScore > 0)
        {
            txtStoryScore.text = $"High score: {storyHighScore}";
        }
        if (endlessHighScore > 0)
        {
            txtEndlessScore.text = $"High score: {endlessHighScore}";
        }
    }

}
