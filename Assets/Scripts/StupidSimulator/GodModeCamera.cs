using UnityEngine;

public class GodModeCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 10f;
    public float fastSpeed = 30f;
    public float smoothSpeed = 10f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public bool requiresRightClickToLook = true; // Si es true, hay que mantener click derecho para rotar

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 targetPosition;

    void OnEnable()
    {
        targetPosition = transform.position;
        rotationX = transform.localEulerAngles.x;
        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        // 1. Manejo de Rotación (Mirar alrededor)
        if (!requiresRightClickToLook || Input.GetMouseButton(1))
        {
            if (requiresRightClickToLook && Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            rotationY += Input.GetAxis("Mouse X") * lookSensitivity;
            rotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
        else if (requiresRightClickToLook && Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 2. Manejo de Movimiento (WASD + Q/E)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
        if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
        
        // Q y E para subir y bajar verticalmente
        if (Input.GetKey(KeyCode.E)) moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) moveDirection -= Vector3.up;

        // Normalizamos para no ir más rápido en diagonal
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();

        // Calculamos la nueva posición objetivo
        targetPosition += moveDirection * currentSpeed * Time.deltaTime;

        // Suavizamos el movimiento final
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
