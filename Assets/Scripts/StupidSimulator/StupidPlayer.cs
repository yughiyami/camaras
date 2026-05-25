using UnityEngine;
using UnityEngine.AI;

public class StupidPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 150f; // Ajustado para girar con A y D suavemente
    
    private NavMeshAgent agent;
    private Animator animator;

    private float attackAnimTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Leer input suave
        float horizontal = Input.GetAxis("Horizontal"); // A/D (Izquierda/Derecha)
        float vertical = Input.GetAxis("Vertical");     // W/S (Adelante/Atrás)

        // 1. Rotar sobre su propio eje (Como un volante)
        transform.Rotate(0, horizontal * turnSpeed * Time.deltaTime, 0);

        // 2. Mover hacia ADELANTE o ATRÁS según su propia orientación
        Vector3 moveDirection = transform.forward * vertical;
        
        if (agent != null && agent.enabled)
        {
            agent.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // 3. Animación Inteligente y Hack de Ataque
        if (animator != null)
        {
            animator.speed = 1f; // Reseteamos la velocidad por la limitación de Unity
            
            if (attackAnimTimer > 0)
            {
                // HACK: Forzamos la animación de correr al máximo mientras está quieto atacando
                animator.SetFloat("Vert", moveSpeed * 2f); 
                attackAnimTimer -= Time.deltaTime;
            }
            else
            {
                // Le mandamos la velocidad en positivo para que camine normal
                float speedMag = Mathf.Abs(vertical) * moveSpeed;
                animator.SetFloat("Vert", speedMag);
            }
        }

        // --- SISTEMA BÁSICO DE ATAQUE ---
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            Attack();
        }
    }

    void Attack()
    {
        attackAnimTimer = 0.3f; // Tiempo que dura el "falso ataque" moviendo las patas rápido

        // Detectar enemigos en frente
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1f, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.gameObject != this.gameObject)
            {
                Fighter enemy = hit.GetComponent<Fighter>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1f);
                    Debug.Log("Le pegaste a: " + enemy.gameObject.name);
                }
            }
        }
    }
}
