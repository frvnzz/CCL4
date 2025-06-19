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
    public ScorePopup ScorePopupPrefab;
    public RectTransform scoreTextRect;
    public TMP_Text finalScoreText;

    private DamageVignette damageVignette;

    void Awake()
    {
        damageVignette = FindFirstObjectByType<DamageVignette>();

        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void SetHealth(int health)
    {
        healthText.text = " " + health;
    }

    public void SetAmmo(int current, int total)
    {
        ammoText.text = $"{current}/{total}";
    }

    public void SetScore(int score)
    {
        scoreText.text = " " + score;
    }

    public void ShowGameOver(bool show)
    {
        gameOverPanel.SetActive(show);
        finalScoreText.text = "Final Score: " + scoreText.text;
    }

    public void ShowDamageVignette()
    {
        if (damageVignette != null)
        {
            damageVignette.ShowVignette();
        }
    }

    public void ShowScorePopup(int score)
    {
        if (ScorePopupPrefab != null && scoreTextRect != null)
        {
            var popup = Instantiate(ScorePopupPrefab, scoreTextRect.parent);
            popup.transform.SetAsLastSibling(); // Ensure it's on top
            popup.transform.position = scoreTextRect.position;
            popup.Init(score);
        }
    }
}
