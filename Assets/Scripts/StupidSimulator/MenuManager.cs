using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Selector de Mapas")]
    public string mapa1Nombre = "Mapa_1";
    public string mapa2Nombre = "Mapa_2"; // Deberás crear esta escena
    
    [Header("UI Vistas Previas (Opcional)")]
    public GameObject vistaPreviaMapa1;
    public GameObject vistaPreviaMapa2;

    [Header("Selector de Animales")]
    // Aquí el usuario arrastrará los Toggles y los prefabs de los animales en el Inspector de Unity
    public AnimalSelection[] animalesDisponibles;

    [System.Serializable]
    public class AnimalSelection
    {
        public Toggle toggleUI;
        public GameObject animalPrefab;
    }

    void Start()
    {
        // Al iniciar el menú, mostramos la vista previa del mapa 1 por defecto
        SeleccionarMapa1();
    }

    // --- MÉTODOS PARA BOTONES DE MAPA ---
    public void SeleccionarMapa1()
    {
        GlobalSettings.SelectedMapName = mapa1Nombre;
        if(vistaPreviaMapa1 != null) vistaPreviaMapa1.SetActive(true);
        if(vistaPreviaMapa2 != null) vistaPreviaMapa2.SetActive(false);
    }

    public void SeleccionarMapa2()
    {
        GlobalSettings.SelectedMapName = mapa2Nombre;
        if(vistaPreviaMapa1 != null) vistaPreviaMapa1.SetActive(false);
        if(vistaPreviaMapa2 != null) vistaPreviaMapa2.SetActive(true);
    }

    // --- BOTÓN JUGAR ---
    public void Jugar()
    {
        // 1. Limpiamos la lista anterior
        GlobalSettings.SelectedAnimals.Clear();

        // 2. Revisamos qué Toggles están encendidos y agregamos esos animales
        foreach (var seleccion in animalesDisponibles)
        {
            if (seleccion.toggleUI != null && seleccion.toggleUI.isOn)
            {
                if (seleccion.animalPrefab != null)
                {
                    GlobalSettings.SelectedAnimals.Add(seleccion.animalPrefab);
                }
            }
        }

        // Si el jugador no seleccionó ningún animal, le podemos dar una advertencia o agregar uno por defecto
        if (GlobalSettings.SelectedAnimals.Count == 0)
        {
            Debug.LogWarning("No elegiste ningún animal. Asegurate de encender algún Toggle.");
            return; // No carga la escena hasta que elija uno
        }

        // 3. Cargamos la escena que guardamos en SeleccionarMapa
        SceneManager.LoadScene(GlobalSettings.SelectedMapName);
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
