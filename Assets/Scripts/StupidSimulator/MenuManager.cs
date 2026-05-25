using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Asegurate de que el mapa 1 esté en Build Settings (ej. index 1)
    public void LoadMap1()
    {
        SceneManager.LoadScene("Mapa_1");
    }

    // Asegurate de que el mapa 2 esté en Build Settings (ej. index 2)
    public void LoadMap2()
    {
        SceneManager.LoadScene("Mapa_2"); // Cambiar por el nombre real de tu escena
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
