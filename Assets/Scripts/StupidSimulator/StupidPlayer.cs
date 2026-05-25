using UnityEngine;
using UnityEngine.AI;

public class StupidPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 700f;
    
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Leer input (WASD / Flechas)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            // Rotar hacia donde nos movemos
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Mover (usamos transform.Translate o pasamos velocidad al agente para respetar colisiones)
            if (agent != null && agent.enabled)
            {
                agent.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            }
        }

        // Animación
        if (animator != null)
        {
            animator.SetFloat("Vert", moveDirection.magnitude * moveSpeed);
        }

        // --- SISTEMA BÁSICO DE ATAQUE ---
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Asegurate de tener un trigger "Attack" en tu Animator
        }

        // Detectar enemigos en frente
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1f, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.gameObject != this.gameObject)
            {
                Fighter enemy = hit.GetComponent<Fighter>();
                if (enemy != null)
                {
                    enemy.TakeDamage(25f);
                    Debug.Log("Le pegaste a: " + enemy.gameObject.name);
                }
            }
        }
    }
}
