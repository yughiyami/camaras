using UnityEngine;
using UnityEngine.AI;
using System;

public class PossessionManager : MonoBehaviour
{
    // EVENTO PÚBLICO: Avisa a las cámaras cuando tomamos control de un nuevo animal
    public static Action<Transform> OnCharacterPossessed;

    private Fighter currentPossessedFighter;

    void Update()
    {
        // Al hacer click izquierdo, intentamos poseer a un luchador
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Fighter clickedFighter = hit.collider.GetComponent<Fighter>();
                
                if (clickedFighter != null && clickedFighter != currentPossessedFighter)
                {
                    Possess(clickedFighter);
                }
            }
        }
    }

    void Possess(Fighter newFighter)
    {
        // 1. Liberar al luchador anterior (si había uno)
        if (currentPossessedFighter != null)
        {
            currentPossessedFighter.isPlayerControlled = false;
            
            // Le quitamos el control de jugador
            StupidPlayer oldController = currentPossessedFighter.GetComponent<StupidPlayer>();
            if (oldController != null) oldController.enabled = false;

            // Reactivamos que el NavMeshAgent mueva a la IA automáticamente
            NavMeshAgent oldAgent = currentPossessedFighter.GetComponent<NavMeshAgent>();
            if (oldAgent != null)
            {
                oldAgent.ResetPath();
            }
        }

        // 2. Poseer al nuevo
        currentPossessedFighter = newFighter;
        currentPossessedFighter.isPlayerControlled = true;

        // Frenar a la IA
        NavMeshAgent newAgent = currentPossessedFighter.GetComponent<NavMeshAgent>();
        if (newAgent != null)
        {
            newAgent.ResetPath(); // Detiene cualquier movimiento que estuviera haciendo la IA
        }

        // Agregarle o habilitarle el StupidPlayer
        StupidPlayer playerController = currentPossessedFighter.GetComponent<StupidPlayer>();
        if (playerController == null)
        {
            // Si no lo tenía, se lo agregamos dinámicamente
            playerController = currentPossessedFighter.gameObject.AddComponent<StupidPlayer>();
        }
        playerController.enabled = true;

        Debug.Log("Has poseído a: " + newFighter.gameObject.name);

        // Disparamos el evento para que las cámaras reaccionen
        OnCharacterPossessed?.Invoke(currentPossessedFighter.transform);
    }
}
