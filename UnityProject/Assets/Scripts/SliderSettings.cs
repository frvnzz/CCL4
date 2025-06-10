using UnityEngine;
using UnityEngine.UI;


public class SliderSettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public float senseX = 400;
    public float senseY = 400;

    void Start()
    {
        sensitivitySlider.value = senseX;
        sensitivitySlider.value = senseY;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void OnSensitivityChanged(float newValue)
    {
        senseX = newValue;
        senseY = newValue;
    }
}

