using UnityEngine;

public class SpawnCrates : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] Transform[] spawnPoints;

    [Header("Crate Settings")]
    [SerializeField] GameObject[] cratePrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       SpawnRandomCrate();
    }

    void SpawnRandomCrate()
    {
        if (spawnPoints.Length == 0 || cratePrefabs.Length == 0)
        {
            Debug.LogWarning("No spawn points or crate prefabs assigned.");
            return;
        }

        // Select a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Select a random crate prefab
        GameObject cratePrefab = cratePrefabs[Random.Range(0, cratePrefabs.Length)];

        // Instantiate the crate at the selected spawn point
        Instantiate(cratePrefab, spawnPoint.position, Quaternion.identity);
    }

    
}
