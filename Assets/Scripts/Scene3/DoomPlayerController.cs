using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DoomPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 180f;
    public float gravity = -20f;

    private CharacterController _controller;
    private Vector3 _velocity;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        // Opcional: Ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotación Horizontal (Giro tipo tanque)
        float rotateInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * rotateInput * rotationSpeed * Time.deltaTime);

        // Movimiento (Adelante/Atrás únicamente con Vertical)
        // Strafe lateral desactivado para modo clásico puro, o incluido si se desea
        float moveInput = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * moveInput;

        _controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravedad simple
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
