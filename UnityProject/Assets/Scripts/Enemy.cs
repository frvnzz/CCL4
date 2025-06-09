using UnityEngine;

public class Enemy : MonoBehaviour
{
    // This script handles the enemy's collision with the player

    void OnllisionEnter(Collision collision)
    {
        GameManager.instance.TakeDamage(10);
        Debug.Log("Player hit by enemy!"); // This line logs a message to the console when the player is hit by an enemy  
    }
}
