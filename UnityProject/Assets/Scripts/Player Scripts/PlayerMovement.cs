using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private bool isSprinting = false;
    private bool jumpPressed = false;

    //Getter
    public Vector2 MoveInput => moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        var actions = playerInput.actions;
        actions["Move"].performed += OnMove;
        actions["Move"].canceled += OnMove;
        actions["Jump"].performed += OnJump;
        actions["Sprint"].performed += OnSprint;
        actions["Sprint"].canceled += OnSprint;
    }

    void OnDisable()
    {
        var actions = playerInput.actions;
        actions["Move"].performed -= OnMove;
        actions["Move"].canceled -= OnMove;
        actions["Jump"].performed -= OnJump;
        actions["Sprint"].performed -= OnSprint;
        actions["Sprint"].canceled -= OnSprint;
    }

    void FixedUpdate()
    {
        Move(); //Move the player with fixed update
        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }
    }

    //=================INPUT SYSTEM CALLBACKS=================================
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
            AkUnitySoundEngine.SetRTPCValue("player_speed", moveSpeed, null);
        else
            AkUnitySoundEngine.SetRTPCValue("player_speed", 0f, null);

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
        if (isSprinting)
            AkUnitySoundEngine.SetRTPCValue("player_speed", sprintSpeed, null);
        else
            AkUnitySoundEngine.SetRTPCValue("player_speed", moveSpeed, null);

    }

    //================MOVEMENT FUNCTIONS===============================================
    void Move() //Applies the velocity to the rigidbody to make the player move
    {
        float speed = isSprinting ? sprintSpeed : moveSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = velocity;
    }

    void Jump() //Makes the player jump as long as grounded
    {
        if (IsGrounded())
        {
            AkUnitySoundEngine.PostEvent("Play_jump", gameObject);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded() //Checks for collision with object downwards (still needs ground layer check)
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
