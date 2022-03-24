using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIGameOver : MonoBehaviour
{
    private int multiplier = 1;
    private int totalScore;
    private Dictionary<MoveObjectType, int> collectedShrooms;

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text highScoreText;
    [SerializeField]
    private GameObject record;
    // [SerializeField]
    // private Text multiplierText;

    [SerializeField]
    private List<CollectedShroomTexts> shroomCountTexts;

    // Start is called before the first frame update
    void Start()
    {
        shroomCountTexts.ForEach(x =>
        {
            x.countText.text = "x " + 0.ToString();
        });
    }

    // Update is called once per frame
    void Update()
    {
        // multiplierText.text = multiplier.ToString();

    }

    public void UpdateScore(int multiplier, int totalScore, Dictionary<MoveObjectType, int> collectedShrooms)
    {
        // this.multiplier = multiplier;
        this.totalScore = totalScore;
        this.collectedShrooms = collectedShrooms;

    }

    public void Show()
    {
        scoreText.text = totalScore.ToString();
        shroomCountTexts.ForEach(x =>
        {
            if (collectedShrooms != null && collectedShrooms.ContainsKey(x.type))
            {
                x.countText.text = "x " + collectedShrooms[x.type].ToString();
            }
        });

        string sceneName = SceneManager.GetActiveScene().name;
        string scoreName = $"highScore_{sceneName}";
        int score = PlayerPrefs.GetInt(scoreName, 0);
        Debug.Log($"Finding {scoreName} and it's {score}!");
        if (totalScore > score)
        {
            Debug.Log($"writing {scoreName} as {totalScore}!");
            PlayerPrefs.SetInt(scoreName, totalScore);
            record.SetActive(true);
        }
        highScoreText.text = $"{(score > 0 ? score : "-")}";
    }
}

[Serializable]
public class CollectedShroomTexts
{
    public MoveObjectType type;
    public Text countText;
}
