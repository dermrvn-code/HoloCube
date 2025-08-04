using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] SliderValue gridSizeSlider;
    [SerializeField] SliderValue cubeSpeedSlider;
    [SerializeField] SliderValue playerSpeedSlider;
    [SerializeField] SliderValue pointsSlider;
    [SerializeField] SliderValue obstacleSlider;
    [SerializeField] List<SliderValue> sliders;


    [Header("Display")]
    [SerializeField] CubeDisplay cubeDisplay;

    [Header("Input")]
    [SerializeField] KeyCode enter1 = KeyCode.Space;
    [SerializeField] KeyCode enter2 = KeyCode.Joystick1Button0;



    void Awake()
    {
        LoadValues();
        gridSizeSlider.OnValueChanged += UpdateSize;
        cubeSpeedSlider.OnValueChanged += UpdateCubeSpeed;
        playerSpeedSlider.OnValueChanged += UpdatePlayerSpeed;
        pointsSlider.OnValueChanged += UpdatePointsInterval;
        obstacleSlider.OnValueChanged += UpdateObstacleInterval;

        sliders = new List<SliderValue>
        {
            gridSizeSlider,
            cubeSpeedSlider,
            playerSpeedSlider,
            pointsSlider,
            obstacleSlider
        };
    }

    SliderValue currentSlider;
    int currentIndex = 0;
    static float lastUpdateTime = 0f;
    void Update()
    {
        if (currentSlider != null) currentSlider.selected = false;
        currentSlider = sliders[currentIndex];
        currentSlider.selected = true;

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        // Prevent overshooting by adding a cooldown between keypresses
        float cooldown = 0.2f;
        if (Time.time - lastUpdateTime > cooldown)
        {
            if (Input.GetKeyDown(enter1) || Input.GetKeyDown(enter2))
            {
                Next();
                return;
            }

            if (vertical == 0 && horizontal == 0)
                return;

            if (vertical > 0)
            {
                currentIndex = (currentIndex - 1 + sliders.Count) % sliders.Count;
            }
            else if (vertical < 0)
            {
                currentIndex = (currentIndex + 1) % sliders.Count;
            }

            if (horizontal > 0)
            {
                currentSlider.Value += 1;
            }
            else if (horizontal < 0)
            {
                currentSlider.Value -= 1;
            }

            lastUpdateTime = Time.time;
        }
    }

    public void Next()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
    }

    void LoadValues()
    {
        gridSizeSlider.Value = PlayerPrefs.GetInt("GridSize", 4);
        cubeSpeedSlider.Value = Mathf.RoundToInt(PlayerPrefs.GetFloat("CubeSpeed", 20f));
        playerSpeedSlider.Value = Mathf.RoundToInt(PlayerPrefs.GetFloat("PlayerSpeed", 20f));
        pointsSlider.Value = Mathf.RoundToInt(PlayerPrefs.GetFloat("PointsInterval", 5f));
        obstacleSlider.Value = Mathf.RoundToInt(PlayerPrefs.GetFloat("ObstacleInterval", 15f));
    }

    void UpdateSize(int value)
    {
        cubeDisplay.Size = value;
        PlayerPrefs.SetInt("GridSize", value);
        PlayerPrefs.Save();
    }

    void UpdateCubeSpeed(int value)
    {
        PlayerPrefs.SetFloat("CubeSpeed", value);
        PlayerPrefs.Save();
    }

    void UpdatePlayerSpeed(int value)
    {
        PlayerPrefs.SetFloat("PlayerSpeed", value);
        PlayerPrefs.Save();
    }

    void UpdatePointsInterval(int value)
    {
        PlayerPrefs.SetFloat("PointsInterval", value);
        PlayerPrefs.Save();
    }

    void UpdateObstacleInterval(int value)
    {
        PlayerPrefs.SetFloat("ObstacleInterval", value);
        PlayerPrefs.Save();
    }
}
