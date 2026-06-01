using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Generador de laberinto procedimental para la mecánica de persecución.
/// Genera pasillos de 90 grados utilizando el algoritmo Recursive Backtracker.
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("Dimensiones de la Matriz")]
    public int width = 10;
    public int height = 10;
    
    [Tooltip("El espacio de centro a centro de cada celda. Para un tigre con NavMeshAgent, se recomienda al menos 4-5.")]
    public float cellSize = 4.0f; 

    [Header("Assets Modulares")]
    [Tooltip("Asigna aquí un prefab de pared (ej. Wall_1M o Wall_2M).")]
    public GameObject wallPrefab;

    private const string CONTAINER_NAME = "Maze_Container";

    private class Cell
    {
        public int x, z;
        public bool visited = false;
        // Paredes por celda (solo Norte y Este para evitar duplicados en la matriz)
        public bool northWall = true;
        public bool eastWall = true;

        public Cell(int x, int z) { this.x = x; this.z = z; }
    }

    private Cell[,] grid;

    [ContextMenu("Generar Laberinto")]
    public void GenerateMaze()
    {
        if (wallPrefab == null)
        {
            Debug.LogError("Por favor, asigna un 'wallPrefab' en el Inspector.");
            return;
        }

        // 1. Limpieza de laberintos previos
        GameObject oldMaze = GameObject.Find(CONTAINER_NAME);
        if (oldMaze != null) DestroyImmediate(oldMaze);

        // 2. Crear el contenedor limpio
        GameObject mazeParent = new GameObject(CONTAINER_NAME);
        mazeParent.transform.position = transform.position;

        // 3. Lógica del Algoritmo
        InitializeGrid();
        CarveMaze(0, 0);

        // 4. Instanciación física
        InstantiateMaze(mazeParent.transform);
        
        Debug.Log($"<color=green>Laberinto '{CONTAINER_NAME}' generado ({width}x{height}).</color> No olvides hornear el NavMesh.");
    }

    private void InitializeGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z] = new Cell(x, z);
            }
        }
    }

    // Algoritmo DFS (Recursive Backtracker)
    private void CarveMaze(int x, int z)
    {
        grid[x, z].visited = true;
        List<Cell> neighbors = GetUnvisitedNeighbors(x, z);

        while (neighbors.Count > 0)
        {
            int randomIndex = Random.Range(0, neighbors.Count);
            Cell next = neighbors[randomIndex];
            
            // "Romper" la pared entre la celda actual y la elegida
            if (next.x > x) grid[x, z].eastWall = false; // Derecha
            else if (next.x < x) grid[next.x, next.z].eastWall = false; // Izquierda
            else if (next.z > z) grid[x, z].northWall = false; // Arriba
            else if (next.z < z) grid[next.x, next.z].northWall = false; // Abajo

            CarveMaze(next.x, next.z);
            neighbors = GetUnvisitedNeighbors(x, z);
        }
    }

    private List<Cell> GetUnvisitedNeighbors(int x, int z)
    {
        List<Cell> neighbors = new List<Cell>();
        if (x > 0 && !grid[x - 1, z].visited) neighbors.Add(grid[x - 1, z]);
        if (x < width - 1 && !grid[x + 1, z].visited) neighbors.Add(grid[x + 1, z]);
        if (z > 0 && !grid[x, z - 1].visited) neighbors.Add(grid[x, z - 1]);
        if (z < height - 1 && !grid[x, z + 1].visited) neighbors.Add(grid[x, z + 1]);
        return neighbors;
    }

    private void InstantiateMaze(Transform parent)
    {
        float offset = cellSize / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellCenter = new Vector3(x * cellSize, 0, z * cellSize);

                // Instanciar Pared Norte
                if (grid[x, z].northWall)
                {
                    PlaceWall(cellCenter + new Vector3(0, 0, offset), Quaternion.identity, parent);
                }

                // Instanciar Pared Este
                if (grid[x, z].eastWall)
                {
                    PlaceWall(cellCenter + new Vector3(offset, 0, 0), Quaternion.Euler(0, 90, 0), parent);
                }

                // Cerrar Perímetro Sur (solo en la fila 0)
                if (z == 0)
                {
                    PlaceWall(cellCenter + new Vector3(0, 0, -offset), Quaternion.identity, parent);
                }

                // Cerrar Perímetro Oeste (solo en la columna 0)
                if (x == 0)
                {
                    PlaceWall(cellCenter + new Vector3(-offset, 0, 0), Quaternion.Euler(0, 90, 0), parent);
                }
            }
        }
    }

    private void PlaceWall(Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject wall = Instantiate(wallPrefab, pos, rot, parent);
        
        // Ajuste de escala: Si cellSize es grande, podrías querer escalar el wallPrefab
        // wall.transform.localScale = new Vector3(cellSize, wall.transform.localScale.y, wall.transform.localScale.z);

        // PREPARACIÓN PARA IA:
        // Marcamos el objeto como Navigation Static para que el NavMesh lo reconozca al hornear.
        #if UNITY_EDITOR
        GameObjectUtility.SetStaticEditorFlags(wall, StaticEditorFlags.NavigationStatic);
        #endif
    }
}