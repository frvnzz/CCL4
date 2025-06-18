using UnityEngine;
using UnityEngine.UI;


public class SliderVolume : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        volumeSlider.value = GameManager.instance.MasterVolume;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float newValue)
    {
        GameManager.instance.MasterVolume = newValue;
    }
}