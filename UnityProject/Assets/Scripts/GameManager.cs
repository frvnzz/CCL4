using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // public HealthBar HealthBar;

    public TMP_Text healthText;
    public TMP_Text ammoText;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    private int currentHealth;
    private int currentScore = 0;

    [SerializeField] public int maxHealth = 100; // Maximum health of the player
    [SerializeField] public PlayerController playerController;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(instance);

        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;
        // HealthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // Update ammo and score text
        ammoText.text = playerController.CurrentAmmo + "/" + playerController.TotalAmmo;
        scoreText.text = "Score: " + currentScore;

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthText.text = "Health: " + currentHealth;
        // HealthBar.SetHealth(currentHealth);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }

    public void ResetGame()
    {
        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;
        // HealthBar.SetMaxHealth(maxHealth);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        playerController.enabled = true;

        // Reset player ammo
        playerController.ResetAllAmmo();

        //Reset position
        playerController.transform.position = Vector3.zero; // Reset to a specific position, e.g., origin

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = "Score: " + currentScore;
    }
}
