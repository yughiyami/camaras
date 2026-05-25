using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [Header("Cámaras Virtuales (Arrastrar los GameObjects)")]
    public GameObject godCamera;         // Cámara 1: Vista desde el cielo
    public GameObject thirdPersonCamera; // Cámara 2: Sigue al jugador (Debe ser un Cinemachine FreeLook o Virtual Camera)
    public GameObject firstPersonCamera; // Cámara 3: Anclada a la cabeza del animal

    [Header("Opciones de Control")]
    public KeyCode togglePerspectiveKey = KeyCode.C; // Tecla para cambiar entre 1ra y 3ra
    public KeyCode exitPossessionKey = KeyCode.F;    // Tecla para soltar al animal y volver al cielo

    [Header("Objetivo de Cinemachine")]
    public Transform cameraTargetProxy; 
    public Transform firstPersonProxy; // Nuevo proxy para la 1ra persona

    [Header("Suavizado (Anti-mareo)")]
    public float positionSmoothness = 10f;
    public float rotationSmoothness = 5f;

    private bool isPossessing = false;
    private bool isFirstPerson = false;
    private Transform currentTarget;

    void OnEnable()
    {
        PossessionManager.OnCharacterPossessed += HandleCharacterPossession;
    }

    void OnDisable()
    {
        PossessionManager.OnCharacterPossessed -= HandleCharacterPossession;
    }

    void Start()
    {
        ActivarGodCamera();
    }

    void Update()
    {
        if (isPossessing)
        {
            if (Input.GetKeyDown(togglePerspectiveKey))
            {
                isFirstPerson = !isFirstPerson;
                if (isFirstPerson) ActivarPrimeraPersona();
                else ActivarTerceraPersona();
            }

            if (Input.GetKeyDown(exitPossessionKey))
            {
                ActivarGodCamera();
            }
        }
    }

    // Usamos LateUpdate para seguir al jugador de forma ultra-suave después de que él se mueva
    void LateUpdate()
    {
        if (isPossessing && currentTarget != null)
        {
            // Posición ideal para 3ra persona (Pecho)
            Vector3 targetPos3rd = currentTarget.position + Vector3.up * 1f;
            // Posición ideal para 1ra persona (Ojos)
            Vector3 targetPos1st = currentTarget.position + currentTarget.forward * 0.5f + Vector3.up * 1.5f;

            // Interpolación suave para evitar rotaciones bruscas del NavMeshAgent
            if (cameraTargetProxy != null)
            {
                cameraTargetProxy.position = Vector3.Lerp(cameraTargetProxy.position, targetPos3rd, Time.deltaTime * positionSmoothness);
                cameraTargetProxy.rotation = Quaternion.Slerp(cameraTargetProxy.rotation, currentTarget.rotation, Time.deltaTime * rotationSmoothness);
            }

            if (firstPersonProxy != null)
            {
                firstPersonProxy.position = Vector3.Lerp(firstPersonProxy.position, targetPos1st, Time.deltaTime * positionSmoothness);
                firstPersonProxy.rotation = Quaternion.Slerp(firstPersonProxy.rotation, currentTarget.rotation, Time.deltaTime * rotationSmoothness);
            }
        }
    }

    private void HandleCharacterPossession(Transform newTarget)
    {
        currentTarget = newTarget;
        isPossessing = true;
        isFirstPerson = false; 

        // Al inicio, teletransportamos los proxies para que no viajen desde el otro lado del mapa
        if (cameraTargetProxy != null)
        {
            cameraTargetProxy.position = currentTarget.position + Vector3.up * 1f;
            cameraTargetProxy.rotation = currentTarget.rotation;
        }

        if (firstPersonProxy != null)
        {
            firstPersonProxy.position = currentTarget.position + currentTarget.forward * 0.5f + Vector3.up * 1.5f;
            firstPersonProxy.rotation = currentTarget.rotation;
            
            // La cámara de 1ra persona ahora sigue al proxy suave, no al animal directamente
            firstPersonCamera.transform.SetParent(firstPersonProxy);
            firstPersonCamera.transform.localPosition = Vector3.zero;
            firstPersonCamera.transform.localRotation = Quaternion.identity;
        }

        ActivarTerceraPersona();
    }

    private void ActivarGodCamera()
    {
        isPossessing = false;
        godCamera.SetActive(true);
        thirdPersonCamera.SetActive(false);
        firstPersonCamera.SetActive(false);
    }

    private void ActivarTerceraPersona()
    {
        godCamera.SetActive(false);
        thirdPersonCamera.SetActive(true);
        firstPersonCamera.SetActive(false);
    }

    private void ActivarPrimeraPersona()
    {
        godCamera.SetActive(false);
        thirdPersonCamera.SetActive(false);
        firstPersonCamera.SetActive(true);
    }
}
