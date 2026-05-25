using UnityEngine;
using UnityEngine.AI;

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
        if (animalPrefabs.Length == 0)
        {
            Debug.LogWarning("No asignaste prefabs de animales en el GameManager.");
            return;
        }

        for (int i = 0; i < numberOfAnimalsToSpawn; i++)
        {
            // Elegir un prefab al azar
            GameObject prefabToSpawn = animalPrefabs[Random.Range(0, animalPrefabs.Length)];

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
