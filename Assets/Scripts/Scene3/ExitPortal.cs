using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ExitPortal : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject winPanel;
    public string nextSceneName = "Scene1";

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player") || other.name.Contains("DeerPlayer"))
        {
            _hasTriggered = true;
            StartCoroutine(HandleWinSequence());
        }
    }

    private IEnumerator HandleWinSequence()
    {
        if (winPanel != null) winPanel.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene(nextSceneName);
    }
}
