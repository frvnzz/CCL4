using UnityEngine;

public class AIController : MonoBehaviour
{
    private GameObject destination;
    private UnityEngine.AI.NavMeshAgent agent;

    public event System.Action OnEnemyDefeated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Player");

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(destination.transform.position);
    }

    // Example: Call when enemy "dies"
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NotifyDeath();
            Destroy(gameObject);
        }
    }

    public void NotifyDeath()
    {
        if (OnEnemyDefeated != null)
            OnEnemyDefeated.Invoke();
    }
}
