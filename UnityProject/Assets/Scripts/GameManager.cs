using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // public HealthBar HealthBar;

    public TMP_Text healthText;
    public TMP_Text ammoText;
    public TMP_Text scoreText;

    private int currentHealth;
    
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
        ammoText.text = playerController.currentAmmo + "/" + playerController.maxAmmo;
        // scoreText.text = "Score: " + currentScore;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthText.text = "Health: " + currentHealth;
        // HealthBar.SetHealth(currentHealth);
    }
}
