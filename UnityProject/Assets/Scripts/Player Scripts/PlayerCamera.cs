using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 1f;

    private PlayerInput playerInput;
    private Vector2 lookInput;
    private float cameraPitch = 0f;

    //Getter
    public Transform CameraTransform => cameraTransform;
    public Vector2 LookInput => lookInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
        // Optionally get sensitivity from GameManager if needed
        // mouseSensitivity = GameManager.instance.mouseSensitivity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        var actions = playerInput.actions;
        actions["Look"].performed += OnLook;
        actions["Look"].canceled += OnLook;
    }

    void OnDisable()
    {
        var actions = playerInput.actions;
        actions["Look"].performed -= OnLook;
        actions["Look"].canceled -= OnLook;
    }

    void Update()
    {
        HandleCamera();
    }

    //==================INPUT SYSTEM CALLBACKS=================================
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //==================CAMERA HANDLING========================================
    void HandleCamera()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
