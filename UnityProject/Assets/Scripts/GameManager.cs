using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private TMP_Text healthText;
    private TMP_Text ammoText;
    private TMP_Text scoreText;
    private GameObject gameOverPanel;
    private int currentHealth;
    private int currentScore = 0;

    [SerializeField] public int maxHealth = 100; // Maximum health of the player
    [SerializeField] public float mouseSensitivity = 2f; // Mouse sensitivity for player control
    private WeaponManager weaponManager;
    private GameObject player;
    private Vector3 respawnPosition = new Vector3(377.55f, 62.84f, 601.57f);

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
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            healthText = GameObject.Find("Health Text")?.GetComponent<TMP_Text>();
            ammoText = GameObject.Find("Ammo Text")?.GetComponent<TMP_Text>();
            scoreText = GameObject.Find("Score Text")?.GetComponent<TMP_Text>();
            gameOverPanel = GameObject.Find("GameOverScreen");

            weaponManager = FindFirstObjectByType<WeaponManager>();
            player = GameObject.FindGameObjectWithTag("Player");

            currentHealth = maxHealth;
            healthText.text = "Health: " + currentHealth;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            // Update ammo and score text
            ammoText.text = weaponManager.CurrentAmmo + "/" + weaponManager.TotalAmmo;
            scoreText.text = "Score: " + currentScore;

            if (currentHealth <= 0)
            {
                GameOver();
            }
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
        AkUnitySoundEngine.StopAll();
        Time.timeScale = 0f; // Pause the game
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

        // Reset score
        currentScore = 0;

        // Reset player ammo
        weaponManager.ResetAllAmmo();

        //Reset position
        player.transform.position = respawnPosition; // Reset to a specific position, e.g., origin

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    public void AddScore(int score)
    {
        currentScore += score;
        //scoreText.text = "Score: " + currentScore;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }
}