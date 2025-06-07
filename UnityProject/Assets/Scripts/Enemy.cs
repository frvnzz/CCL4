using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        PlayerHealth.instance.health--;
        
        if (PlayerHealth.instance.health < 0)
        {
            PlayerHealth.instance.health = 0;
        }
        Debug.Log("Health decreased. Current Health: " + PlayerHealth.instance.health);
    }
}
