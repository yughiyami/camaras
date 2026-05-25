using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Fighter : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    public bool isPlayerControlled = false;
    
    private NavMeshAgent agent;
    private Animator animator;

    // Timer para la IA (vagabundear)
    private float wanderTimer;
    public float wanderRadius = 10f;
    public float wanderInterval = 4f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        wanderTimer = wanderInterval;
    }

    void Update()
    {
        if (isPlayerControlled)
        {
            // El jugador lo controla (lógica manejada por PlayerController)
            return;
        }

        // --- LÓGICA DE INTELIGENCIA ARTIFICIAL (Vagabundear) ---
        wanderTimer += Time.deltaTime;
        
        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }

        // Animación basada en la velocidad del NavMeshAgent
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Vert", speed);
        }
    }

    // Encuentra un punto válido en el NavMesh
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Acá podrías reproducir animación de muerte
        Destroy(gameObject);
    }
}
