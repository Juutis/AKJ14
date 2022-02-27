using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    private int multiplier = 1;
    private int totalScore;

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text multiplierText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        multiplierText.text = multiplier.ToString();
        scoreText.text = totalScore.ToString();
    }

    public void UpdateScore(int multiplier, int totalScore)
    {
        this.multiplier = multiplier;
        this.totalScore = totalScore;
    }
}
