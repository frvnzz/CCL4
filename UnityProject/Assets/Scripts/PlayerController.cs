using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    public float sprintSpeed = 10f;
    private bool isSprinting = false;

    [Header("References")]
    public Transform cameraTransform;
    public GameObject hitEffectPrefab;
    public GameObject reloadText;

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
    public GameObject[] gunPrefabs;
    public Transform gunHolder;

    private List<WeaponInstance> weapons = new List<WeaponInstance>();
    private int currentGunIndex = 0;
    private WeaponInstance currentWeapon;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private float cameraPitch = 0f;
    private float scrollInput;

    //gun settings
    private Transform gunTransform;
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

        // Initialize weapons
        if (gunPrefabs != null && gunPrefabs.Length > 0)
        {
            foreach (var prefab in gunPrefabs)
            {
                var stats = prefab.GetComponent<GunStats>();
                weapons.Add(new WeaponInstance(prefab, stats));
            }
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
        actions["Sprint"].performed += OnSprint;
        actions["Sprint"].canceled += OnSprint;
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
        actions["Sprint"].performed -= OnSprint;
        actions["Sprint"].canceled -= OnSprint;
    }

    void Update()
    {
        HandleCamera();
        HandleGunWobble();
        HandleGunSwitch();

        if (currentWeapon != null && currentWeapon.Stats.isAutomatic && isFiring && !isReloading)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Fire();
                fireCooldown = 1f / currentWeapon.Stats.fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        Move();
        // Debug movement
        // Debug.Log("Player Velocity: " + rb.linearVelocity);
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
        if(moveInput != Vector2.zero)
            AkUnitySoundEngine.SetRTPCValue("player_speed", moveSpeed, null);
        else
            AkUnitySoundEngine.SetRTPCValue("player_speed", 0f, null);

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

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
        if (isSprinting)
            AkUnitySoundEngine.SetRTPCValue("player_speed", sprintSpeed, null);
        else
            AkUnitySoundEngine.SetRTPCValue("player_speed", moveSpeed, null);

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if (currentWeapon.Stats.isAutomatic)
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
        scrollInput = context.ReadValue<Vector2>().y;
    }

    void Move()
    {
        float speed = isSprinting ? sprintSpeed : moveSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        if (IsGrounded())
        {
            AkUnitySoundEngine.PostEvent("Play_jump", gameObject);
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
        if (isReloading || currentWeapon.CurrentAmmo <= 0) return;
        currentWeapon.CurrentAmmo--;
        if (currentWeapon.Stats.fireEventName != null)
        {
            AkUnitySoundEngine.PostEvent(currentWeapon.Stats.fireEventName, gameObject);
        }

        if (currentWeapon.Stats.muzzleFlash != null)
            currentWeapon.Stats.muzzleFlash.Play();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, currentWeapon.Stats.fireRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<AIController>()?.TakeDamage(currentWeapon.Stats.damage);

                GameObject effect = Instantiate(
                    hitEffectPrefab,
                    hit.point,
                    Quaternion.LookRotation(hit.normal),
                    hit.collider.transform
                );
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(effect, ps.main.duration);
                else
                    Destroy(effect, 2f);

                if (hitmarkerCoroutine != null)
                    StopCoroutine(hitmarkerCoroutine);
                hitmarkerCoroutine = StartCoroutine(ShowHitmarker(hitmarkerDuration));
            }
        }
        else
        {
            Debug.Log("Missed!");
        }
        fireCooldown = 1f / currentWeapon.Stats.fireRate;
        gunKnockbackOffset = currentWeapon.Stats.gunKnockbackAmount;

        if (currentWeapon.CurrentAmmo <= 0 && reloadText != null)
        {
            reloadText.SetActive(true);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void HandleGunWobble()
    {
        if (gunTransform == null) return;

        gunSwayOffset = Vector3.Lerp(gunSwayOffset, Vector3.zero, Time.deltaTime * gunSwaySpeed);
        gunSwayOffset += new Vector3(lookInput.x, lookInput.y, 0) * gunSwayAmount;
        gunSwayOffset = Vector3.ClampMagnitude(gunSwayOffset, gunSwayAmount * 2f);

        if (moveInput.sqrMagnitude > 0.01f)
            gunBobTimer += Time.deltaTime * gunBobSpeed;
        else
            gunBobTimer = 0;

        float bobOffset = Mathf.Sin(gunBobTimer) * gunBobAmount;
        Vector3 bob = new Vector3(0, bobOffset, 0);

        gunKnockbackOffset = Mathf.Lerp(gunKnockbackOffset, 0f, Time.deltaTime * gunKnockbackDecay);
        Vector3 knockback = new Vector3(0, 0, -gunKnockbackOffset);

        Vector3 targetPos = gunInitialLocalPos + gunSwayOffset + bob + knockback;
        gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetPos, Time.deltaTime * gunSwaySpeed);
    }

    private IEnumerator Reload()
    {
        if (isReloading || currentWeapon.CurrentAmmo == currentWeapon.Stats.maxAmmo) yield break;
        isReloading = true;

        float elapsed = 0f;
        float moveDuration = currentWeapon.Stats.reloadTime * 0.4f;
        Vector3 startPos = gunTransform.localPosition;
        Vector3 downPos = gunInitialLocalPos + Vector3.down * 1f;

        if (currentWeapon.Stats.reloadEventName != null)
        {
            AkUnitySoundEngine.PostEvent(currentWeapon.Stats.reloadEventName, gameObject);
        }

        while (elapsed < moveDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(startPos, downPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = downPos;

        yield return new WaitForSeconds(currentWeapon.Stats.reloadTime * 0.2f);

        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(downPos, gunInitialLocalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = gunInitialLocalPos;

        int ammoNeeded = currentWeapon.Stats.maxAmmo - currentWeapon.CurrentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentWeapon.TotalAmmo);
        currentWeapon.CurrentAmmo += ammoToReload;
        currentWeapon.TotalAmmo -= ammoToReload;

        isReloading = false;
        reloadText.SetActive(false);
    }

    void HandleGunSwitch()
    {
        if (scrollInput > 0f)
        {
            int next = (currentGunIndex + 1) % weapons.Count;
            EquipGun(next);
        }
        else if (scrollInput < 0f)
        {
            int prev = (currentGunIndex - 1 + weapons.Count) % weapons.Count;
            EquipGun(prev);
        }
        scrollInput = 0f;
    }

    void EquipGun(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        if (currentWeapon != null && currentWeapon.GunObject != null)
            Destroy(currentWeapon.GunObject);

        var weapon = weapons[index];
        weapon.GunObject = Instantiate(weapon.Prefab, gunHolder);
        weapon.Stats = weapon.GunObject.GetComponent<GunStats>();

        gunTransform = weapon.GunObject.transform;
        gunInitialLocalPos = gunTransform.localPosition;

        currentGunIndex = index;
        currentWeapon = weapon;
    }

    IEnumerator ShowHitmarker(float duration)
    {
        if (hitmarker != null)
        {
            hitmarker.SetActive(true);
            AkUnitySoundEngine.PostEvent("Play_Hitmarker_Sound_Effect", gameObject);
            yield return new WaitForSeconds(duration);
            hitmarker.SetActive(false);
        }
    }



    public int CurrentAmmo => currentWeapon != null ? currentWeapon.CurrentAmmo : 0;
    public int TotalAmmo => currentWeapon != null ? currentWeapon.TotalAmmo : 0;

    public void ResetAllAmmo()
    {
        foreach (var weapon in weapons)
        {
            weapon.ResetAmmo();
        }
    }

    public void AddAmmo(GameObject weaponPrefab, int amount)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.Prefab == weaponPrefab)
            {
                int maxTotal = weapon.Stats != null ? weapon.Stats.maxTotalAmmo : 9999;
                weapon.TotalAmmo = Mathf.Min(weapon.TotalAmmo + amount, maxTotal);
                break;
            }
        }
    }
}