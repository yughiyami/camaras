using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeerVision : MonoBehaviour
{
    [Header("Camera & Render")]
    public Camera mapCamera;
    public RectTransform mapUI;
    public UnityEngine.UI.Image cooldownFill;

    [Header("Settings")]
    public float fullscreenDuration = 3f;
    public float cooldownDuration = 8f;
    public float lerpSpeed = 0.4f;

    private bool _isExpanded = false;
    private bool _isOnCooldown = false;
    private Vector2 _minimizedSize = new Vector2(200, 200);
    private Vector2 _minimizedAnchor;

    private void Start()
    {
        if (mapUI != null) _minimizedAnchor = mapUI.anchoredPosition;
        if (mapCamera != null) mapCamera.enabled = true;
        if (cooldownFill != null) cooldownFill.fillAmount = 1f;
    }

    private void Update()
    {
        if (mapCamera != null)
        {
            Vector3 targetPos = transform.position;
            targetPos.y = 40f;
            mapCamera.transform.position = targetPos;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_isExpanded && !_isOnCooldown)
            {
                StartCoroutine(ToggleVision(true));
            }
            else if (_isExpanded)
            {
                StopAllCoroutines();
                StartCoroutine(ToggleVision(false));
            }
        }
    }

    private IEnumerator ToggleVision(bool expand)
    {
        _isExpanded = expand;
        float elapsed = 0f;

        Vector2 startSize = mapUI.sizeDelta;
        Vector2 targetSize = expand ? new Vector2(Screen.width, Screen.height) : _minimizedSize;
        
        Vector2 startPos = mapUI.anchoredPosition;
        Vector2 targetPos = expand ? Vector2.zero : _minimizedAnchor;

        Vector2 startMin = mapUI.anchorMin;
        Vector2 startMax = mapUI.anchorMax;
        Vector2 startPivot = mapUI.pivot;

        Vector2 targetMin = expand ? new Vector2(0.5f, 0.5f) : new Vector2(1, 1);
        Vector2 targetMax = expand ? new Vector2(0.5f, 0.5f) : new Vector2(1, 1);
        Vector2 targetPivot = expand ? new Vector2(0.5f, 0.5f) : new Vector2(1, 1);

        Time.timeScale = expand ? 0.3f : 1.0f;

        while (elapsed < lerpSpeed)
        {
            float t = elapsed / lerpSpeed;
            mapUI.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            mapUI.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            mapUI.anchorMin = Vector2.Lerp(startMin, targetMin, t);
            mapUI.anchorMax = Vector2.Lerp(startMax, targetMax, t);
            mapUI.pivot = Vector2.Lerp(startPivot, targetPivot, t);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        mapUI.sizeDelta = targetSize;
        mapUI.anchoredPosition = targetPos;
        mapUI.anchorMin = targetMin;
        mapUI.anchorMax = targetMax;
        mapUI.pivot = targetPivot;

        if (expand)
        {
            yield return new WaitForSecondsRealtime(fullscreenDuration);
            if (_isExpanded) StartCoroutine(ToggleVision(false));
        }
        else
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;
        float elapsed = 0f;
        while (elapsed < cooldownDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (cooldownFill != null) cooldownFill.fillAmount = elapsed / cooldownDuration;
            yield return null;
        }
        if (cooldownFill != null) cooldownFill.fillAmount = 1f;
        _isOnCooldown = false;
    }
}
