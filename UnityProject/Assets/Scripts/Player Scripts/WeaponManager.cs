using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public GameObject[] gunPrefabs;
    public Transform gunHolder;
    public GameObject reloadText;
    public GameObject hitEffectPrefab;
    public GameObject hitmarker;

    [Header("Hit Settings")]
    public float hitmarkerDuration = 0.2f;

    // Internal state
    private List<WeaponInstance> weapons = new List<WeaponInstance>();
    private int currentGunIndex = 0;
    private WeaponInstance currentWeapon;
    private bool isReloading = false;
    private bool isFiring = false;
    private float fireCooldown = 0f;
    private float scrollInput;
    private Coroutine hitmarkerCoroutine;

    // Dependencies
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private WeaponWobble weaponWobble;

    public int CurrentAmmo => currentWeapon != null ? currentWeapon.CurrentAmmo : 0;
    public int TotalAmmo => currentWeapon != null ? currentWeapon.TotalAmmo : 0;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        weaponWobble = GetComponent<WeaponWobble>();

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
        actions["Attack"].performed += OnFire;
        actions["Attack"].canceled += OnFire;
        actions["Reload"].performed += OnReload;
        actions["ScrollWheel"].performed += OnScrollWheel;
    }

    void OnDisable()
    {
        var actions = playerInput.actions;
        actions["Attack"].performed -= OnFire;
        actions["Attack"].canceled -= OnFire;
        actions["Reload"].performed -= OnReload;
        actions["ScrollWheel"].performed -= OnScrollWheel;
    }

    void Update()
    {
        HandleGunSwitch();

        if (currentWeapon != null && currentWeapon.Stats.isAutomatic && isFiring && !isReloading) // Handle automatic firing
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Fire();
                fireCooldown = 1f / currentWeapon.Stats.fireRate;
            }
        }
    }

    //==================INPUT SYSTEM CALLBACKS=================================
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
        // Debug.Log("ScrollWheel: " + context.ReadValue<Vector2>().y + "; Sprinting: " + isSprinting);
        scrollInput = context.ReadValue<Vector2>().y;
    }

    //==================WEAPON HANDLING=========================================
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

        Ray ray = new Ray(playerCamera.CameraTransform.position, playerCamera.CameraTransform.forward);
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

            else if (hit.collider.CompareTag("Explosive"))
            {
                hit.collider.GetComponent<Explosive>()?.Explode();
            }
        }
        else
        {
            Debug.Log("Missed!");
        }
        fireCooldown = 1f / currentWeapon.Stats.fireRate;
        weaponWobble.AddKnockback(currentWeapon.Stats.gunKnockbackAmount);

        if (currentWeapon.CurrentAmmo <= 0 && reloadText != null)
        {
            reloadText.SetActive(true);
        }
    }

    private IEnumerator Reload()
    {
        if (isReloading || currentWeapon.CurrentAmmo == currentWeapon.Stats.maxAmmo || currentWeapon.TotalAmmo <= 0) yield break;
        isReloading = true;
        reloadText.SetActive(currentWeapon.CurrentAmmo <= 0);

        if (currentWeapon.Stats.reloadEventName != null)
        {
            AkUnitySoundEngine.PostEvent(currentWeapon.Stats.reloadEventName, gameObject);
        }

        // Play reload animation using WeaponWobble
        yield return StartCoroutine(weaponWobble.PlayReloadAnimation(currentWeapon.Stats.reloadTime));

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

        weaponWobble.Init(weapon.GunObject.transform, playerCamera, playerMovement);

        currentGunIndex = index;
        currentWeapon = weapon;

        reloadText.SetActive(currentWeapon.CurrentAmmo <= 0);
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


    //=============Functions for Ammo Management=========================
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
