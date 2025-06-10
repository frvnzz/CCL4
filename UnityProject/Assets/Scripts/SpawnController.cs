using TMPro;
using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] Transform[] spawnPoints; // Array of spawn points
    // [SerializeField] GameObject[] objectsToSpawn; // Array of objects to spawn
    //[SerializeField] int maxSpawnCount = 50; // Maximum number of objects to spawn

    [Header("Difficulty Settings")]
    [SerializeField] float spawnInterval = 5f; // Time interval between spawns
    [SerializeField] int difficultyLevel = 1; // Difficulty level for scaling spawn rate
    //[SerializeField] int[] enemiesPerWave;

    [Header("Text")]
    [SerializeField] TMP_Text waveText;

    //int enemiesThisWave;
    //private int currentSpawnCount = 0; // Current number of spawned objects

    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemies; // Prefabs to spawn in this wave
        public float spawnDelay = 1f; // Delay between spawns in this wave
    }

    [Header("Waves")]
    [SerializeField] Wave[] waves;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;

    void Start()
    {
        if (waves.Length > 0)
        {
            StartCoroutine(HandleWave(currentWaveIndex));
        }
    }

    IEnumerator HandleWave(int waveIndex)
    {
        Wave wave = waves[waveIndex];
        waveText.text = $"Wave: {waveIndex + 1}";

        for (int i = 0; i < wave.enemies.Length; i++)
        {
            SpawnEnemy(wave.enemies[i]);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        // Wait until all enemies are defeated
        while (enemiesAlive > 0)
        {
            yield return null;
        }

        // Next wave
        currentWaveIndex++;
        if (currentWaveIndex < waves.Length)
        {
            StartCoroutine(HandleWave(currentWaveIndex));
        }
        else
        {
            waveText.text = "All Waves Complete!";
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;

        // Listen for enemy death (assumes enemies call OnEnemyDefeated on death)
        AIController notifier = enemy.GetComponent<AIController>();
        if (notifier != null)
        {
            notifier.OnEnemyDefeated += OnEnemyDefeated;
        }
    }

    void OnEnemyDefeated()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
    }
}