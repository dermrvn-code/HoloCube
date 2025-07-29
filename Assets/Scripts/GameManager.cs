using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 10;
    public float spacing = 0.2f;
    public Vector2Int startPosition = new Vector2Int(0, 0);
    public FaceController.FaceType startFace = FaceController.FaceType.Right;

    [Header("Speed Settings")]
    public float playerSpeed = 20f;
    public float cubeSpeed = 20f;

    [Header("Score Settings")]
    public int points = 10;
    public int startPoints = 2;

    [Header("Spawn Intervals")]
    public float pointSpawnInterval = 5f;
    public float obstacleSpawnInterval = 15f;

    [Header("UI References")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject countdown;

    [NonSerialized] public bool isRunning = false;
    [NonSerialized] public bool isGameOver = false;

    TMP_Text countdownText;
    GameOverScreen gameOverScreen;
    bool newHighscore = false;
    float startTimeOffset;
    const int startCountdown = 3;
    int score = 0;
    int highScore = 0;

    void Awake()
    {
        if (startPosition == Vector2Int.zero)
        {
            int center = Mathf.FloorToInt(gridSize / 2);
            startPosition = new Vector2Int(center, center);
        }

        countdownText = countdown.GetComponentInChildren<TMP_Text>();
        gameOverScreen = FindObjectOfType<GameOverScreen>();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();

        ResetDisplay();
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        for (int i = startCountdown; i >= 0; i--)
        {
            countdownText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }

        countdown.SetActive(false);
        isRunning = true;
        startTimeOffset = Time.timeSinceLevelLoad;
    }

    void Update()
    {
        if (!isRunning) return;
        UpdateTime();
    }

    public void IncreaseScore()
    {
        score += points;
        if (score > highScore)
        {
            SetNewHighscore(score);
        }
        scoreText.text = score.ToString();
    }

    void SetNewHighscore(int newScore)
    {
        highScore = newScore;
        newHighscore = true;
        highScoreText.text = highScore.ToString();
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    public void ResetDisplay()
    {
        score = 0;
        scoreText.text = "0";
        timeText.text = "00:00:00";
    }

    void UpdateTime()
    {
        timeText.text = FormatTime(Time.timeSinceLevelLoad - startTimeOffset);
    }

    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int millis = (int)((time - (int)time) * 100);
        return $"{minutes:00}:{seconds:00}:{millis:00}";
    }

    public void GameOver()
    {
        isGameOver = true;
        isRunning = false;
        float elapsedTime = Time.timeSinceLevelLoad - startTimeOffset;
        gameOverScreen.GameOver(score, highScore, elapsedTime, newHighscore, Restart);
    }

    void Restart()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
    }

    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
        highScoreText.text = highScore.ToString();
    }
}
