using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public enum GameState
{
    Preparation,
    Battle
}

public class GameManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; }

    [Header("Spawning Settings")]
    public GameObject[] animalPrefabs; // Asignar los prefabs de animals_FREE acá
    public int numberOfAnimalsToSpawn = 30; // Más animales
    public float spawnRadius = 10f; // Más apretados

    void Start()
    {
        CurrentState = GameState.Preparation; // Arranca congelado
        SpawnAnimals();
    }

    void Update()
    {
        // Apretar ENTER para empezar a pelear
        if (CurrentState == GameState.Preparation && Input.GetKeyDown(KeyCode.Return))
        {
            CurrentState = GameState.Battle;
            Debug.Log("<color=red>¡EMPIEZA LA BATALLA!</color>");
        }
    }

    void SpawnAnimals()
    {
        // 1. Tomamos los animales que el usuario eligió en el menú
        List<GameObject> animalesElegidos = GlobalSettings.SelectedAnimals;

        // 2. Si entramos directo a la escena (sin pasar por el menú) y la lista está vacía, usamos los del inspector por defecto
        if (animalesElegidos == null || animalesElegidos.Count == 0)
        {
            if (animalPrefabs.Length > 0)
            {
                animalesElegidos = new List<GameObject>(animalPrefabs);
            }
            else
            {
                Debug.LogWarning("No hay animales elegidos ni prefabs por defecto.");
                return;
            }
        }

        for (int i = 0; i < numberOfAnimalsToSpawn; i++)
        {
            // Elegir un prefab al azar de la lista elegida
            GameObject prefabToSpawn = animalesElegidos[Random.Range(0, animalesElegidos.Count)];

            // Buscar un punto válido en el NavMesh cerca del GameManager
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
            {
                Instantiate(prefabToSpawn, hit.position, Quaternion.identity);
            }
        }
    }
}
