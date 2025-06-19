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
    
    [Header("Crate Spawn Settings")]
    [SerializeField] Transform[] crateSpawnPoints;

    [Header("Crate Settings")]
    [SerializeField] int cratesPerWave; // Set how many crates you want per wave
    [SerializeField] GameObject[] cratePrefabs;

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
            //Play the wave start sound

            infiniteWaveCount++;
            if (infiniteWaveCount > 1)
            {
                AkUnitySoundEngine.PostEvent("Play_ZombieWave", gameObject);
                yield return StartCoroutine(AnimateWaveText(infiniteWaveCount));
            }

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
        Transform crateSpawnPoint = crateSpawnPoints[Random.Range(0, crateSpawnPoints.Length)];

        // Select a random crate prefab
        GameObject cratePrefab = cratePrefabs[Random.Range(0, cratePrefabs.Length)];

        // Instantiate the crate at the selected spawn point
        Instantiate(cratePrefab, crateSpawnPoint.position, Quaternion.identity);
    }

        IEnumerator AnimateWaveText(int waveNumber)
    {
        RectTransform rect = waveText.GetComponent<RectTransform>();

        // Store original position and anchors
        Vector2 originalAnchorMin = rect.anchorMin;
        Vector2 originalAnchorMax = rect.anchorMax;
        Vector2 originalAnchoredPosition = rect.anchoredPosition;

        // Store original color and alpha
        Color originalColor = waveText.color;
        float originalAlpha = originalColor.a;
        Color whiteColor = new Color(1f, 1f, 1f, originalAlpha);

        // Move to center top (anchor at top center, position zero) and fade color to white
        Vector2 topCenterAnchor = new Vector2(0.5f, 1f);
        Vector2 topCenterPosition = Vector2.zero;
        float moveDuration = 0.5f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            rect.anchorMin = Vector2.Lerp(originalAnchorMin, topCenterAnchor, t);
            rect.anchorMax = Vector2.Lerp(originalAnchorMax, topCenterAnchor, t);
            rect.anchoredPosition = Vector2.Lerp(originalAnchoredPosition, topCenterPosition, t);

            // Fade color from original to white (alpha stays original)
            Color lerpedColor = Color.Lerp(originalColor, whiteColor, t);
            lerpedColor.a = originalAlpha;
            waveText.color = lerpedColor;
            yield return null;
        }
        rect.anchorMin = topCenterAnchor;
        rect.anchorMax = topCenterAnchor;
        rect.anchoredPosition = topCenterPosition;
        waveText.color = whiteColor;

        waveText.text = $"{waveNumber}";
        waveText.gameObject.SetActive(true);

        // Smooth flash 3 times (only alpha fades, color stays white)
        int flashCount = 3;
        float fadeDuration = 0.2f;
        float visibleDuration = 0.3f;

        for (int i = 0; i < flashCount; i++)
        {
            // Fade in alpha
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                float currentAlpha = Mathf.Lerp(0f, originalAlpha, t);
                waveText.color = new Color(1f, 1f, 1f, currentAlpha);
                yield return null;
            }
            waveText.color = new Color(1f, 1f, 1f, originalAlpha);
            yield return new WaitForSeconds(visibleDuration);

            // Fade out alpha
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                float currentAlpha = Mathf.Lerp(originalAlpha, 0f, t);
                waveText.color = new Color(1f, 1f, 1f, currentAlpha);
                yield return null;
            }
            waveText.color = new Color(1f, 1f, 1f, 0f);
        }

        // Final fade in alpha and hold (color stays white)
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float currentAlpha = Mathf.Lerp(0f, originalAlpha, t);
            waveText.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        waveText.color = new Color(1f, 1f, 1f, originalAlpha);
        yield return new WaitForSeconds(0.5f);

        // Animate back to original position (keep color white, alpha original)
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            rect.anchorMin = Vector2.Lerp(topCenterAnchor, originalAnchorMin, t);
            rect.anchorMax = Vector2.Lerp(topCenterAnchor, originalAnchorMax, t);
            rect.anchoredPosition = Vector2.Lerp(topCenterPosition, originalAnchoredPosition, t);
            waveText.color = new Color(1f, 1f, 1f, originalAlpha);
            yield return null;
        }
        rect.anchorMin = originalAnchorMin;
        rect.anchorMax = originalAnchorMax;
        rect.anchoredPosition = originalAnchoredPosition;
        waveText.color = new Color(1f, 1f, 1f, originalAlpha);

        // Fade color from white back to original color (alpha stays original)
        t = 0f;
        float colorFadeDuration = 0.5f;
        while (t < 1f)
        {
            t += Time.deltaTime / colorFadeDuration;
            Color lerped = Color.Lerp(whiteColor, originalColor, t);
            lerped.a = originalAlpha;
            waveText.color = lerped;
            yield return null;
        }
        Color reset = originalColor;
        reset.a = originalAlpha;
        waveText.color = reset;
    }
}