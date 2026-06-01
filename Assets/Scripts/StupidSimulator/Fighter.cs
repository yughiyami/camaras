using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Fighter : MonoBehaviour
{
    public float maxHealth = 5f; // Vida de cada animal
    private float currentHealth;
    
    public bool isPlayerControlled = false;
    
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Combate IA")]
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public float aggroRadius = 15f; // Distancia para empezar a perseguir
    private float lastAttackTime;
    private Fighter targetEnemy;
    private float attackAnimTimer = 0f;

    [Header("Vagabundear")]
    public float wanderRadius = 10f;
    public float wanderInterval = 4f;
    private float wanderTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        wanderTimer = wanderInterval; // Empieza listo para moverse
        
        // Evita que los animales intenten ocupar exactamente el mismo pixel y se traben
        if (agent != null)
        {
            agent.stoppingDistance = attackRange - 0.5f; 
        }
    }

    void Update()
    {
        if (isPlayerControlled)
        {
            // El jugador lo controla
            return;
        }

        // Si estamos en fase de preparación, nos quedamos quietos
        if (GameManager.CurrentState == GameState.Preparation)
        {
            if (agent.hasPath) agent.ResetPath();
            UpdateAnimation();
            return;
        }

        // --- LÓGICA DE INTELIGENCIA ARTIFICIAL ---
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            // MODO COMBATE: Tenemos un enemigo a la vista
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance <= attackRange)
            {
                // Golpear
                Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                direction.y = 0; 
                
                if (direction.sqrMagnitude > 0.001f) 
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                }

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack(targetEnemy);
                }
            }
            else
            {
                // Perseguir
                if (Vector3.Distance(agent.destination, targetEnemy.transform.position) > 1f)
                {
                    agent.SetDestination(targetEnemy.transform.position);
                }
            }
        }
        else
        {
            // MODO PACÍFICO: Vagabundear porque no hay nadie cerca
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(newPos);
                }
                wanderTimer = 0;
            }
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator != null && agent != null && agent.isActiveAndEnabled)
        {
            if (attackAnimTimer > 0)
            {
                animator.SetFloat("Vert", 10f); 
                attackAnimTimer -= Time.deltaTime;
            }
            else
            {
                float speed = agent.velocity.magnitude;
                animator.SetFloat("Vert", speed);
            }
        }
    }

    void FindClosestEnemy()
    {
        Fighter[] allFighters = Object.FindObjectsByType<Fighter>(FindObjectsInactive.Exclude);
        float closestDistance = Mathf.Infinity;
        targetEnemy = null;

        foreach (Fighter potentialTarget in allFighters)
        {
            if (potentialTarget == this || !potentialTarget.enabled) continue; // No contar muertos ni a sí mismo

            float distanceToTarget = Vector3.Distance(transform.position, potentialTarget.transform.position);
            
            // SOLO lo fijamos como objetivo si está dentro del radio de agresión
            if (distanceToTarget < closestDistance && distanceToTarget <= aggroRadius)
            {
                closestDistance = distanceToTarget;
                targetEnemy = potentialTarget;
            }
        }
    }

    void Attack(Fighter target)
    {
        lastAttackTime = Time.time;
        attackAnimTimer = 0.3f;
        target.TakeDamage(1f);
    }

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

        // HACK VISUAL DE DAÑO: El animal se achica un 10% con cada golpe
        transform.localScale *= 0.9f;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // HACK VISUAL DE MUERTE: Tumbar al animal de lado
        transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, 0f);
        
        // Apagamos los componentes para que sea un cadáver inerte
        if (agent != null) agent.enabled = false;
        if (animator != null) animator.enabled = false;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        this.enabled = false; // Apaga este script para que no busque enemigos
        
        // Desaparece después de 4 segundos
        Destroy(gameObject, 4f);
    }
}
