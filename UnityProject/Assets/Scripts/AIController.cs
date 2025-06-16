using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] float attackRange = 2.0f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackDelay = 2.0f;
    [SerializeField] float speed = 3.5f;
    [SerializeField] int health = 100;

    private GameObject destination;
    private UnityEngine.AI.NavMeshAgent agent;
    private LineRenderer lineRenderer;

    public event System.Action OnEnemyDefeated;

    private bool invulnerable = false;

    [SerializeField] public Animator animator;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private bool isWalking;
    public float despawnEnemyTime;
    //private bool isAttacking;



    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = attackRange;

        animator = GetComponentInChildren<Animator>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        SetRagdollActive(false); 

        // lineRenderer = gameObject.AddComponent<LineRenderer>();
        // lineRenderer.positionCount = 2;
        // lineRenderer.startWidth = 0.05f;
        // lineRenderer.endWidth = 0.05f;
        // lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        // lineRenderer.startColor = Color.red;
        // lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        agent.SetDestination(destination.transform.position);

        // Visualize the ray constantly
        // Vector3 start = transform.position;
        // Vector3 end = start + transform.forward * attackRange;
        // lineRenderer.SetPosition(0, start);
        // lineRenderer.SetPosition(1, end);

        // Check if agent is in attack range

        bool inAttackRange = attackRange > agent.remainingDistance;
        animator.SetBool("Attacking", inAttackRange);

        //Debug.Log("In Attack Range: " + inAttackRange + ", Remaining Distance: " + agent.remainingDistance);
        if (!inAttackRange)
        {
            isWalking = true;
            animator.SetBool("Walking", isWalking);
        }
        else
        {
            isWalking = false;
            animator.SetBool("Walking", isWalking);
        }
        // Debug.Log("Walking: " + isWalking);

        AttackPlayer(inAttackRange);
    }

    public void NotifyDeath()
    {
        if (OnEnemyDefeated != null)
            OnEnemyDefeated.Invoke();
    }

    public void AttackPlayer(bool inAttackRange)
    {
        if (!inAttackRange) return; // Only attack if in range

        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, attackRange))
        {
            // Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {


                if (invulnerable) return;

                invulnerable = true;


                GameManager.instance.TakeDamage(attackDamage);

                StartCoroutine(DamageDelay()); // Delay to simulate attack animation
            }
        }
    }

    public void DestroyEnemy()
    {
        SetRagdollActive(true);
        NotifyDeath();
        GameManager.instance.AddScore(100); //Add score for defeating the enemy
        Destroy(gameObject, despawnEnemyTime);
    }

    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        invulnerable = false;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private void SetRagdollActive(bool active)
    {
        foreach (var rb in ragdollBodies)
        {
            if (rb != agent) // Don't affect the main Rigidbody
                rb.isKinematic = !active;
        }
        foreach (var col in ragdollColliders)
        {
            if (col != GetComponent<Collider>()) // Don't affect the main Collider
                col.enabled = active;
        }
        // Optionally disable Animator when ragdoll is active
        animator.enabled = !active;
        agent.enabled = !active;

        // Disable the main CapsuleCollider when ragdoll is active
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        capsule.enabled = !active;
        }
}


