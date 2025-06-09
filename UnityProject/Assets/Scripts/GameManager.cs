using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public HealthBar HealthBar;

    public int currentHealth;
    
    [SerializeField] public int maxHealth = 100; // Maximum health of the player


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
        DontDestroyOnLoad(this);

        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // This method can be used to check for player input or other game events
        // For example, you could check if the player presses a key to take damage
        if (Input.GetKeyDown(KeyCode.K)) // Press Space to simulate taking damage
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.SetHealth(currentHealth);
    }
}
