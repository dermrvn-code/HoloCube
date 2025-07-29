using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    GameObject gameOverPanel;

    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    TMP_Text highScoreText;

    [SerializeField]
    TMP_Text timeText;

    [SerializeField]
    TMP_Text countdownText;

    [SerializeField]
    GameObject newHighScorePanel;

    [SerializeField]
    GameObject inGameOverlay;

    void Start()
    {
        SetActive(false);
        newHighScorePanel.SetActive(false);
        inGameOverlay.SetActive(true);
    }

    void SetActive(bool active)
    {
        gameOverPanel.GetComponent<Image>().enabled = active;

        foreach (Transform child in gameOverPanel.transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    public void GameOver(int score, int highScore, float time, bool newHighscore, Action restart)
    {
        SetActive(true);
        inGameOverlay.SetActive(false);

        scoreText.text = "Score: " + score.ToString();
        highScoreText.text = "Highscore: " + highScore.ToString();
        timeText.text = GameManager.FormatTime(time);

        if (newHighscore)
        {
            newHighScorePanel.SetActive(true);
        }

        StartCoroutine(Countdown(10, () =>
        {
            restart?.Invoke();
        }));
    }

    IEnumerator Countdown(int start, Action onComplete)
    {
        for (int i = start; i > 0; i--)
        {
            countdownText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }
        onComplete?.Invoke();
    }
}
