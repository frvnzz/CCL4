using UnityEngine;
using UnityEngine.InputSystem;

public class CamRotation : MonoBehaviour
{

    public float senseX;
    public float senseY;
    public InputActionReference lookAction;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void OnEnable()
    {
        if (lookAction != null)
            lookAction.action.Enable();
    }

    private void OnDisable()
    {
        if (lookAction != null)
            lookAction.action.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // LookDelta is the mouse movement
        Vector2 lookDelta = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;

        float mouseX = lookDelta.x * Time.deltaTime * senseX;
        float mouseY = lookDelta.y * Time.deltaTime * senseY;

        // Apply the mouse movement to the camera rotation
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp the xRotation to prevent flipping the camera upside down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the camera and orientation
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
