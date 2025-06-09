using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed;

    public float groundDrag;

    public float jumpForce;

    public float jumpCoolDown; // This line controls how long the player has to wait before jumping again

    bool readyToJump = true;

    public float airMultiplier; // This line controls how fast the player can move in the air
    [Header("Ground Check")] public float playerHeight;

    public LayerMask whatIsGround; // This line represents the ground layer
    bool grounded; // This line checks if the player is on the ground

    public Transform orientation;

    public InputActionReference moveAction; 
    public InputActionReference jumpAction;

    Vector2 moveInput;
    Vector3 moveDirection;


    Rigidbody rb;


    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        if (jumpAction != null) jumpAction.action.performed += OnJump;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        if (jumpAction != null) jumpAction.action.performed -= OnJump;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //RigidBody is applied so that it doesn't fall through the ground
        rb.freezeRotation = true;
    }

    void Update()
    {
        // grounded variable uses a raycast to check if the player is touching the ground
        // Raycast measures the half of the player's height from the center point of the player to the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        // This line makes sure that the player doesn't move too fast
        SpeedControl();

        // To avoid sliding, we need to apply drag when the player is grounded
        if (grounded)
        {
            // linearDamping is used to slow down the player 
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }

    

    // FixedUpdate is used when working with physics on rigidbody. As MovePlayer uses AddForce, it is better to use FixedUpdate
    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // This line makes sure that the player moves according to the orientation it is facing
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        // This line makes sure that the player moves when it is grounded
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // This line makes sure that the player moves when it is in the air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    // This varibale helps to limit the player's speed, so that it doesn't exceed the moveSpeed value set in the inspector
    private void SpeedControl()
    {
        // This line makes sure that the player moves only on the X and Z axis, so that it doesn't move up or down
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z); 
        }
    }

    
    // This function is called when the jump action is performed
    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        // This line makes sure that player jumps only when 1. the player is ready to jump, and 2. the player is grounded
        if (readyToJump && grounded)
        {
            readyToJump = false;

            Jump(); 

            // This line makes sure that the player can jump again after a cooldown
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }
    

    private void Jump()
    {
        // This line resets the Y velocity to 0 so that the player jumps the exact same height every time
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // This line applies the force to the player in the upward direction
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
