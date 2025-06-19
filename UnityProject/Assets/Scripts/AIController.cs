using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] float attackRange = 2.0f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackDelay = 1.0f;
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

    private bool isAttacking;
    private bool wasWalking = false;
    public float despawnEnemyTime;
    //private bool isAttacking;
    RaycastHit hit;

    private bool isDead = false;

    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        //agent.stoppingDistance = attackRange;

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
        if (isDead) return;
        agent.SetDestination(destination.transform.position);

        // Visualize the ray constantly
        // Vector3 start = transform.position;
        // Vector3 end = start + transform.forward * attackRange;
        // lineRenderer.SetPosition(0, start);
        // lineRenderer.SetPosition(1, end);
        //Debug.Log("In Attack Range: " + inAttackRange + ", Remaining Distance: " + agent.remainingDistance);
        if (!isAttacking)
        {
            isWalking = true;
            animator.SetBool("Walking", isWalking);

            if (!wasWalking)
            {
                AkUnitySoundEngine.PostEvent("Play_footsteps_enemies", gameObject);
            }
        }
        else
        {
            isWalking = false;
            animator.SetBool("Walking", isWalking);
            AkUnitySoundEngine.PostEvent("Stop_footsteps_enemies", gameObject);
        }
        // Debug.Log("Walking: " + isWalking);
        wasWalking = isWalking;

        CheckAttackRange();

        ContinueMovement();
    }

    public void NotifyDeath()
    {
        if (OnEnemyDefeated != null)
            OnEnemyDefeated.Invoke();
    }

    private void CheckAttackRange()
    {

        if (isDead) return;
        Vector3 direction = transform.forward;


        if (Physics.Raycast(transform.position, direction, out hit, attackRange) && hit.collider.CompareTag("Player"))
        {
            agent.stoppingDistance = attackRange;
            isAttacking = true;
            animator.SetBool("Attacking", isAttacking);
            StartCoroutine(DelayAttack());
        }
        else
        {
            isAttacking = false;
            animator.SetBool("Attacking", isAttacking);
        }
    }

    public void AttackPlayer()
    {

        Vector3 direction = transform.forward;


        if (Physics.Raycast(transform.position, direction, out hit, attackRange))
        {
            // Debug.Log("Raycast hit: " + hit.collider.name);
            // if a player is within "attackRange", then take damage from the player
            if (hit.collider.CompareTag("Player"))
            {
                if (invulnerable) return;
                invulnerable = true;
                StartCoroutine(DamageDelay());
                if (isAttacking) GameManager.instance.TakeDamage(attackDamage);
                Debug.Log("Player hit! Dealing damage: " + attackDamage);
            }
        }
    }

    public void DestroyEnemy()
    {
        isDead = true;
        AkUnitySoundEngine.StopAll(gameObject);
        AkUnitySoundEngine.PostEvent("Play_enemy_death", gameObject);
        SetRagdollActive(true);
        NotifyDeath();
        GameManager.instance.AddScore(100); //Add score for defeating the enemy
        Destroy(gameObject, despawnEnemyTime);
    }

    IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(2f); // Delay to simulate attack animation
        AttackPlayer();
        // isAttacking = false;
        // animator.SetBool("Attacking", isAttacking);
    }

    IEnumerator DamageDelay()
    {
        AkUnitySoundEngine.PostEvent("Play_enemy_attack", gameObject);
        yield return new WaitForSeconds(attackDelay);
        invulnerable = false;
        isAttacking = false;
        animator.SetBool("Attacking", isAttacking);
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

    private void ContinueMovement()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Vector3 direction = (destination.transform.position - transform.position).normalized;
            direction.y = 0; // Only rotate on the Y axis

            if (direction != Vector3.zero)
            {
                // Smoothly rotate towards the player
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                float rotationSpeed = 5f; // You can adjust this value for faster/slower rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}


