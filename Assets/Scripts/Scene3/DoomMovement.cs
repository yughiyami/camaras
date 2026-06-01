using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DoomMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 9f;
    public float mouseSensitivity = 100f;
    public float gravity = -9.81f;

    [Header("Camera Bob")]
    public Transform fpsCamera;
    public float bobFrequency = 10f;
    public float bobAmplitude = 0.05f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _defaultY;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        
        if (fpsCamera != null)
        {
            _defaultY = fpsCamera.localPosition.y;
        }
    }

    void Update()
    {
        // 1. Rotation (Mouse X only)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // 2. Movement (WASD Strafe)
        float x = Input.GetAxis("Horizontal"); // A/D Strafe
        float z = Input.GetAxis("Vertical");   // W/S Forward/Back

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        
        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * currentSpeed * Time.deltaTime);

        // 3. Gravity
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Small downward force to stay grounded
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // 4. Camera Bob
        if (fpsCamera != null)
        {
            bool isMoving = _controller.velocity.magnitude > 0.1f && _controller.isGrounded;
            Vector3 camPos = fpsCamera.localPosition;
            
            if (isMoving)
            {
                camPos.y = _defaultY + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            }
            else
            {
                camPos.y = Mathf.Lerp(camPos.y, _defaultY, Time.deltaTime * 5f);
            }
            
            fpsCamera.localPosition = camPos;
        }
    }
}
