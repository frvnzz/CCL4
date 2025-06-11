using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;

    [Header("References")]
    public Transform cameraTransform;

    [Header("Gun Wobble Settings")]
    private Vector3 gunInitialLocalPos;
    public float gunSwayAmount = 0.05f;
    public float gunSwaySpeed = 4f;
    public float gunBobAmount = 0.02f;
    public float gunBobSpeed = 8f;
    private float gunBobTimer;
    private Vector3 gunSwayOffset;
    public float gunKnockbackDecay = 10f;
    private float gunKnockbackOffset = 0f;

    [Header("Weapons")]
    private int currentGunIndex = 0;
    public GameObject[] gunPrefabs;
    public Transform gunHolder;
    private GameObject currentGunObject;
    private GunStats currentGunStats;


    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private float cameraPitch = 0f;
    private float scrollInput;


    //gun settings
    private Transform gunTransform;
    private float reloadTime;
    public int maxAmmo;
    private float fireRange;
    private float gunKnockbackAmount;
    public int currentAmmo;
    private bool isReloading = false;
    private bool isFiring = false;
    private float fireCooldown = 0f;

    public GameObject hitmarker;
    private Coroutine hitmarkerCoroutine;
    public float hitmarkerDuration = 0.2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gunPrefabs != null && gunPrefabs.Length > 0)
        {
            EquipGun(0);
        }
    }

    void OnEnable()
    {
        var actions = playerInput.actions;
        actions["Move"].performed += OnMove;
        actions["Move"].canceled += OnMove;
        actions["Look"].performed += OnLook;
        actions["Look"].canceled += OnLook;
        actions["Jump"].performed += OnJump;
        actions["Attack"].performed += OnFire;
        actions["Attack"].canceled += OnFire;
        actions["Reload"].performed += OnReload;
        actions["ScrollWheel"].performed += OnScrollWheel;
    }

    void OnDisable()
    {
        var actions = playerInput.actions;
        actions["Move"].performed -= OnMove;
        actions["Move"].canceled -= OnMove;
        actions["Look"].performed -= OnLook;
        actions["Look"].canceled -= OnLook;
        actions["Jump"].performed -= OnJump;
        actions["Attack"].performed -= OnFire;
        actions["Attack"].canceled -= OnFire;
        actions["Reload"].performed -= OnReload;
        actions["ScrollWheel"].performed -= OnScrollWheel;
    }

    void Update()
    {
        HandleCamera();
        HandleGunWobble();
        HandleGunSwitch();

        if (currentGunStats != null && currentGunStats.isAutomatic && isFiring && !isReloading)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Fire();
                fireCooldown = 1f / currentGunStats.fireRate; // fireRate = shots per second
            }
        }
    }

    void FixedUpdate()
    {
        Move();
        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }
    }

    // Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (currentGunStats == null) return;

        if (currentGunStats.isAutomatic)
        {
            if (context.performed)
                isFiring = true;
            else if (context.canceled)
                isFiring = false;
        }
        else
        {
            if (context.performed)
                Fire();
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartCoroutine(Reload());
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        // If you use Vector2, use context.ReadValue<Vector2>().y
        scrollInput = context.ReadValue<Vector2>().y;
    }

    void Move()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCamera()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Fire()
    {
        if (isReloading || currentAmmo <= 0) return;
        currentAmmo--;

        if (currentGunStats.muzzleFlash != null)
            currentGunStats.muzzleFlash.Play();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, fireRange))
        {
            // Debug.Log("Hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<AIController>()?.TakeDamage(currentGunStats.damage);
                Debug.Log("Enemy hit!");

                if (hitmarkerCoroutine != null)
                    StopCoroutine(hitmarkerCoroutine);
                hitmarkerCoroutine = StartCoroutine(ShowHitmarker(hitmarkerDuration));
            }

        }
        else
        {
            Debug.Log("Missed!");
        }
        fireCooldown = 1f / currentGunStats.fireRate;
        gunKnockbackOffset = gunKnockbackAmount;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void HandleGunWobble()
    {
        if (gunTransform == null) return;

        // --- SWAY ---
        // Use look delta for sway, and decay back to zero
        gunSwayOffset = Vector3.Lerp(gunSwayOffset, Vector3.zero, Time.deltaTime * gunSwaySpeed);
        gunSwayOffset += new Vector3(lookInput.x, lookInput.y, 0) * gunSwayAmount;
        gunSwayOffset = Vector3.ClampMagnitude(gunSwayOffset, gunSwayAmount * 2f); // Clamp sway so it doesn't go too far

        // --- BOB ---
        if (moveInput.sqrMagnitude > 0.01f)
            gunBobTimer += Time.deltaTime * gunBobSpeed;
        else
            gunBobTimer = 0;

        float bobOffset = Mathf.Sin(gunBobTimer) * gunBobAmount;
        Vector3 bob = new Vector3(0, bobOffset, 0);

        // --- KNOCKBACK ---
        gunKnockbackOffset = Mathf.Lerp(gunKnockbackOffset, 0f, Time.deltaTime * gunKnockbackDecay);
        Vector3 knockback = new Vector3(0, 0, -gunKnockbackOffset);

        // --- APPLY ---
        Vector3 targetPos = gunInitialLocalPos + gunSwayOffset + bob + knockback;
        gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetPos, Time.deltaTime * gunSwaySpeed);
    }

    private IEnumerator Reload()
    {
        if (isReloading || currentAmmo == maxAmmo) yield break;
        isReloading = true;

        // Animate gun down
        float elapsed = 0f;
        float moveDuration = reloadTime * 0.4f;
        Vector3 startPos = gunTransform.localPosition;
        Vector3 downPos = gunInitialLocalPos + Vector3.down * 0.7f; // How far to move down

        // Move gun down
        while (elapsed < moveDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(startPos, downPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = downPos;

        // Wait in down position
        yield return new WaitForSeconds(reloadTime * 0.2f);

        // Move gun back up
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(downPos, gunInitialLocalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = gunInitialLocalPos;

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void HandleGunSwitch()
    {
        if (scrollInput > 0f)
        {
            int next = (currentGunIndex + 1) % gunPrefabs.Length;
            EquipGun(next);
        }
        else if (scrollInput < 0f)
        {
            int prev = (currentGunIndex - 1 + gunPrefabs.Length) % gunPrefabs.Length;
            EquipGun(prev);
        }
        scrollInput = 0f;
    }

    void EquipGun(int index)
    {
        if (index < 0 || index >= gunPrefabs.Length) return;

        // Destroy previous gun
        if (currentGunObject != null)
            Destroy(currentGunObject);

        // Instantiate new gun
        currentGunObject = Instantiate(gunPrefabs[index], gunHolder);
        currentGunStats = currentGunObject.GetComponent<GunStats>();

        // Set stats from prefab
        currentAmmo = currentGunStats.maxAmmo;
        reloadTime = currentGunStats.reloadTime;
        maxAmmo = currentGunStats.maxAmmo;
        fireRange = currentGunStats.fireRange;
        gunKnockbackAmount = currentGunStats.gunKnockbackAmount;

        // For gun wobble, etc.
        gunTransform = currentGunObject.transform;
        gunInitialLocalPos = gunTransform.localPosition;

        currentGunIndex = index;
    }

    IEnumerator ShowHitmarker(float duration)
    {
        if (hitmarker != null)
        {
            hitmarker.SetActive(true);
            yield return new WaitForSeconds(duration);
            hitmarker.SetActive(false);
        }
    }
}