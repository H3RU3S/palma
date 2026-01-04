using UnityEngine;

public class FlyCamera : MonoBehaviour {
    public float speed = 30f; // Velocità normale
    public float sprintMultiplier = 3f; // Velocità quando premi Shift
    public float sensitivity = 2f;

    void Update() {
         if (Input.GetKeyDown(KeyCode.C)) {
        GetComponent<Camera>().enabled = !GetComponent<Camera>().enabled;
        }
        if (Input.GetMouseButton(1)) {
            float rotateX = Input.GetAxis("Mouse X") * sensitivity;
            float rotateY = -Input.GetAxis("Mouse Y") * sensitivity;
            transform.Rotate(0, rotateX, 0, Space.World);
            transform.Rotate(rotateY, 0, 0, Space.Self);
        }

        // Calcolo velocità attuale
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= sprintMultiplier;

        // Movimento WASD
        float moveForward = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        
        transform.Translate(moveSide, 0, moveForward);
    }
}
