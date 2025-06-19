using UnityEngine;
using UnityEngine.UI;

public class DamageVignette : MonoBehaviour
{
    public Image vignetteImage;
    public float fadeDuration = 0.5f;
    public float maxAlpha = 0.6f;

    private float currentAlpha = 0f;
    private float fadeSpeed;

    void Start()
    {
        SetAlpha(0f);
    }

    void Update()
    {
        if (currentAlpha > 0f)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;
            SetAlpha(Mathf.Max(currentAlpha, 0f));
        }
    }

    public void ShowVignette()
    {
        currentAlpha = maxAlpha;
        fadeSpeed = maxAlpha / fadeDuration;
        SetAlpha(currentAlpha);
    }

    void SetAlpha(float alpha)
    {
        var color = vignetteImage.color;
        color.a = alpha;
        vignetteImage.color = color;
    }
}