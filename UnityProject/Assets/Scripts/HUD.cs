using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [Header("UI References")]
    public TMP_Text healthText;
    public TMP_Text ammoText;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void SetHealth(int health)
    {
        healthText.text = "Health: " + health;
    }

    public void SetAmmo(int current, int total)
    {
        ammoText.text = $"{current}/{total}";
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void ShowGameOver(bool show)
    {
        gameOverPanel.SetActive(show);
    }
}
