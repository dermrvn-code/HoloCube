using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;



    public int Value
    {
        get => Mathf.RoundToInt(slider.value);
        set
        {
            slider.value = value < slider.minValue ? slider.minValue : value > slider.maxValue ? slider.maxValue : value;
            UpdateValueText();
        }
    }

    public bool selected = false;
    void Update()
    {
        selectorImage.enabled = selected;
    }

    public Action<int> OnValueChanged;

    Image selectorImage;
    void Start()
    {
        selectorImage = GetComponent<Image>();

        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
        }
        if (slider == null)
        {
            Debug.LogError("Slider component not found on " + gameObject.name);
            return;
        }
        UpdateValueText();

        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnValueChanged?.Invoke(Value);
    }

    public void OnSliderValueChanged(float _)
    {
        UpdateValueText();
        OnValueChanged?.Invoke(Value);
    }

    void UpdateValueText()
    {
        if (valueText == null) return;
        valueText.text = Value.ToString();
    }
}
