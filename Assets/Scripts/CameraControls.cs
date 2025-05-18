using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform cameraHolder; // assign your CameraHolder here
    public float mouseSensitivity = 2f;
    public float yRotationLimit = 45f;
    public float xRotationLimit = 80f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public bool dead;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {   
        if(dead) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Horizontal (Y-axis) rotation, clamped
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -yRotationLimit, yRotationLimit);

        // Vertical (X-axis) rotation, clamped
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xRotationLimit, xRotationLimit);

        // Apply rotation
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); // rotate player left/right
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // rotate camera up/down
    }
}
