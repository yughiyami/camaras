using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject[] animalPrefabs; // Asignar los prefabs de animals_FREE acá
    public int numberOfAnimalsToSpawn = 15;
    public float spawnRadius = 25f;

    void Start()
    {
        SpawnAnimals();
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
