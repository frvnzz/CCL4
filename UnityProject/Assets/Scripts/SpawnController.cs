using TMPro;
using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] Transform[] spawnPoints; // Array of spawn points
    [SerializeField] float spawnInterval = 1f; // Time interval between spawns
    [SerializeField] float waveCooldown = 5f; // Cooldown time between waves
    [SerializeField] int initialEnemyCount = 5; // Number of enemies in the first wave
    [SerializeField] float enemyScalingFactor = 1.2f; // Factor to scale enemy count per wave
    [SerializeField] int maxEnemiesAlive = 50; // Maximum number of zombies alive at any time

    [Header("Text")]
    [SerializeField] TMP_Text waveText; // Text to display the current wave number

    [Header("Enemy Settings")]
    [SerializeField] GameObject[] enemyPrefabs; // Array of enemy prefabs for randomization

    private int infiniteWaveCount = 0; // Counter for infinite waves
    private int enemiesAlive = 0; // Number of enemies currently alive


    [Header("Spawn Settings")]
    [SerializeField] Transform[] crateSpawnPoints;

    [Header("Crate Settings")]
    [SerializeField] GameObject[] cratePrefabs;
    [SerializeField] int cratesPerWave; // Set how many crates you want per wave

    void Start()
    {
        StartCoroutine(HandleInfiniteWaves()); // Start infinite wave spawning
    }

    IEnumerator HandleInfiniteWaves()
    {
        while (true)
        {
            // Wait until all enemies are defeated before starting the next wave
            while (enemiesAlive > 0)
            {
                yield return null;
            }

            // Cooldown between waves
            yield return new WaitForSeconds(waveCooldown);

            infiniteWaveCount++;
            waveText.text = $"{infiniteWaveCount}";

            for (int i = 0; i < cratesPerWave; i++)
            {
                SpawnRandomCrate();
            }

            // Calculate the number of enemies for the current wave based on scaling
            int enemyCount = Mathf.RoundToInt(initialEnemyCount * Mathf.Pow(enemyScalingFactor, infiniteWaveCount - 1));
            enemyCount = Mathf.Min(enemyCount, maxEnemiesAlive); // Ensure we don't exceed the max limit

            for (int i = 0; i < enemyCount; i++)
            {
                if (enemiesAlive < maxEnemiesAlive) // Only spawn if below the max limit
                {
                    SpawnRandomEnemy(); // Spawn a random enemy
                    yield return new WaitForSeconds(spawnInterval); // Wait before spawning the next enemy
                }
            }
        }
    }

    void SpawnRandomEnemy()
    {
        // Spawn a random enemy prefab from the array
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedEnemy = enemyPrefabs[randomIndex];

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject enemy = Instantiate(selectedEnemy, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;

        // Subscribe to the enemy's defeat event to track remaining enemies
        AIController notifier = enemy.GetComponent<AIController>();
        if (notifier != null)
        {
            notifier.OnEnemyDefeated += OnEnemyDefeated;
        }
    }

    void OnEnemyDefeated()
    {
        // Decrease the count of alive enemies when one is defeated
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
    }

    void SpawnRandomCrate()
    {
        // Select a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Select a random crate prefab
        GameObject cratePrefab = cratePrefabs[Random.Range(0, cratePrefabs.Length)];

        // Instantiate the crate at the selected spawn point
        Instantiate(cratePrefab, spawnPoint.position, Quaternion.identity);
    }
}