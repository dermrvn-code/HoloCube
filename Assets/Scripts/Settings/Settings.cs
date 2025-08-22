using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Settings : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] SliderValue gridSizeSlider;
    [SerializeField] SliderValue cubeSpeedSlider;
    [SerializeField] SliderValue playerSpeedSlider;
    [SerializeField] SliderValue pointsSlider;
    [SerializeField] SliderValue obstacleSlider;
    [SerializeField] SliderValue mirrorSlider;
    [SerializeField] List<SliderValue> sliders;


    [Header("Display")]
    [SerializeField] CubeDisplay cubeDisplay;



    void Awake()
    {
        LoadValues();
        gridSizeSlider.OnValueChanged += UpdateSize;
        cubeSpeedSlider.OnValueChanged += UpdateCubeSpeed;
        playerSpeedSlider.OnValueChanged += UpdatePlayerSpeed;
        pointsSlider.OnValueChanged += UpdatePointsInterval;
        obstacleSlider.OnValueChanged += UpdateObstacleInterval;
        mirrorSlider.OnValueChanged += UpdateMirrorInterval;


        sliders = new List<SliderValue>
        {
            gridSizeSlider,
            cubeSpeedSlider,
            playerSpeedSlider,
            pointsSlider,
            obstacleSlider,
            mirrorSlider
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

        InputAction moveAction = InputSystem.actions.FindAction("Move");
        if (moveAction == null)
        {
            Debug.LogError("Settings: Move action not found in Input System.");
            return;
        }
        Vector2 movement = moveAction.ReadValue<Vector2>();

        float horizontal = Mathf.Round(movement.x);
        float vertical = Mathf.Round(movement.y);

        // Prevent overshooting by adding a cooldown between keypresses
        float cooldown = 0.2f;
        if (Time.time - lastUpdateTime > cooldown)
        {
            InputAction submitAction = InputSystem.actions.FindAction("Submit");

            if (submitAction.IsPressed())
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

    void UpdateMirrorInterval(int value)
    {
        PlayerPrefs.SetInt("Mirror", value);
        PlayerPrefs.Save();
    }
}
