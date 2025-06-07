using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    [SerializeField] public int health = 100; // Maximum health of the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }
}
