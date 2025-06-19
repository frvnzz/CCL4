using UnityEngine;
using UnityEngine.UI;


public class SliderSettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public float sense = 2f;

    void Start()
    {
        sense = GameManager.instance.mouseSensitivity;
        sensitivitySlider.value = sense;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void OnSensitivityChanged(float newValue)
    {
        sense = newValue;
        GameManager.instance.mouseSensitivity = sense;
    }
}