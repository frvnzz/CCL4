using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int currentHealth;
    private int currentScore = 0;

    [SerializeField] public int maxHealth = 100; // Maximum health of the player
    [SerializeField] public float mouseSensitivity = 2f; // Mouse sensitivity for player control
    [SerializeField] private float masterVolume = 100f;
    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;
            AkUnitySoundEngine.SetRTPCValue("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }
    }

    private bool limitEnemySpawns = false;

    public bool LimitEnemySpawns
    {
        get => limitEnemySpawns;
        set
        {
            limitEnemySpawns = value;
        }
    }

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

        // Load persisted volume if available
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            MasterVolume = masterVolume;
        }
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
        Time.timeScale = 1f; // Ensure time scale is reset when a new scene is loaded

        if (scene.name == "Level1")
        {
            weaponManager = FindFirstObjectByType<WeaponManager>();
            player = GameObject.FindGameObjectWithTag("Player");

            currentHealth = maxHealth;
            currentScore = 0;
            HUD.instance.SetHealth(currentHealth);
            HUD.instance.SetScore(currentScore);
            HUD.instance.SetAmmo(weaponManager.CurrentAmmo, weaponManager.TotalAmmo);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            HUD.instance.SetAmmo(weaponManager.CurrentAmmo, weaponManager.TotalAmmo);
            HUD.instance.SetScore(currentScore);

            if (currentHealth <= 0)
            {
                GameOver();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HUD.instance.SetHealth(currentHealth);
        HUD.instance.ShowDamageVignette();
    }

    public void GameOver()
    {
        HUD.instance.ShowGameOver(true);
        AkUnitySoundEngine.StopAll();
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.DeactivateInput(); // Deactivate player input
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }

    public void ResetGame()
    {
        currentHealth = maxHealth;
        HUD.instance.SetHealth(currentHealth);
        HUD.instance.ShowGameOver(false);
        Time.timeScale = 1f; // Resume the game

        // Reset score
        currentScore = 0;

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        HUD.instance.ShowScorePopup(score);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
    }
}