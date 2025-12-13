using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    public float gravity = -9.81f;

    [Header("Salto")]
    public float jumpHeight = 1.5f;   // altezza del salto

    private float xRotation = 0f;
    private CharacterController controller;
    private Camera cam;

    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- Movimento WASD ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // --- Fix per scalini ---
        controller.stepOffset = controller.isGrounded ? 0.4f : 0f;

        // --- Se a terra ---
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            // --- SALTO ---
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Formula fisica corretta
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // --- Gravità ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
