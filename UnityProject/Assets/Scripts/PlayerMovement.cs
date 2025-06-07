using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed;

    public float groundDrag;

    public float jumpForce;

    // This line controls how long the player has to wait before jumping again
    public float jumpCoolDown;

    bool readyToJump = true;

    // This line controls how fast the player can move in the air
    public float airMultiplier; 
    [Header("Ground Check")] public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

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

        Debug.Log("Grounded: " + grounded);
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        // This line makes sure that the player doesn't move too fast
        SpeedControl();

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

    

    // FixedUpdate is used for physics calculations
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

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    
    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        // This line makes sure that player jumps only when 1. the key is pressed, 2. the player is ready to jump, and 3. the player is grounded
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
        // This line makes sure that the player jumps the same height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // This line applies the force to the player in the upward directionx
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
