using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrompt : MonoBehaviour
{
    [SerializeField] private Image failImage;

    private Slider slider;

    private void Awake()
    {
        TryGetComponent(out slider);
    }

    public void InitSlider(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = 0;
    }

    public void IncreaseSliderValue()
    {
        slider.value += Time.deltaTime;
    }
}
