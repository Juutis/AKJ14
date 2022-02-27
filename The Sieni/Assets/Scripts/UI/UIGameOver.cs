using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    private int multiplier = 1;
    private int totalScore;
    private Dictionary<MoveObjectType, int> collectedShrooms;

    [SerializeField]
    private Text scoreText;
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
        scoreText.text = totalScore.ToString();
        shroomCountTexts.ForEach(x =>
        {
            if (collectedShrooms != null && collectedShrooms.ContainsKey(x.type))
            {
                x.countText.text = "x " + collectedShrooms[x.type].ToString();
            }
        });
    }

    public void UpdateScore(int multiplier, int totalScore, Dictionary<MoveObjectType, int> collectedShrooms)
    {
        // this.multiplier = multiplier;
        this.totalScore = totalScore;
        this.collectedShrooms = collectedShrooms;
    }
}

[Serializable]
public class CollectedShroomTexts
{
    public MoveObjectType type;
    public Text countText;
}
