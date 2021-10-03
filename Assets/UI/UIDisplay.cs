using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIDisplay : MonoBehaviour
{
    [SerializeField] int score;
    [SerializeField] TextMesh scoreText;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.color = Color.red;

        scoreText.text = "0000";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
    public void AddScore(int scoreToAdd, Color color)
    {
        score += scoreToAdd;
        scoreText.color = color;
        UpdateScore();
    }
}
