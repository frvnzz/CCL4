using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] float attackRange = 2.0f;
    [SerializeField] float attackDamage = 10.0f;

    private GameObject destination;
    private UnityEngine.AI.NavMeshAgent agent;
    private LineRenderer lineRenderer;

    public event System.Action OnEnemyDefeated;

    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Add and configure LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        agent.SetDestination(destination.transform.position);

        // Visualize the ray constantly
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * attackRange;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        AttackPlayer();
    }

    public void NotifyDeath()
    {
        if (OnEnemyDefeated != null)
            OnEnemyDefeated.Invoke();
    }

    public void AttackPlayer()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, attackRange))
        {
            // Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player hit!");
                // Reduce player's health
            }
        }
    }

    public void DestroyEnemy()
    {
        NotifyDeath();
        Destroy(gameObject);
    }
}
