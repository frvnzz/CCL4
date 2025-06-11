using UnityEngine;

public class GunStats : MonoBehaviour
{
    [Tooltip("Amount of ammo the gun can hold in a single magazine.")]
    public int maxAmmo = 10;
    [Tooltip("Total ammo available for the gun at the start.")]
    public int startingTotalAmmo = 90;
    [Tooltip("Maximum total ammo the gun can hold with ammo pickups.")]
    public int maxTotalAmmo = 240;
    [Tooltip("Time it takes to reload the gun.")]
    public float reloadTime = 1.5f;
    [Tooltip("Range of the weapons's fire.")]
    public float fireRange = 50f;
    [Tooltip("Amount of knockback applied to the player when firing the weapon.")]
    public float gunKnockbackAmount = 0.1f;
    [Tooltip("Is the gun automatic? If true, it will fire continuously while the fire button is held down.")]
    public bool isAutomatic = false;
    [Tooltip("Rate of fire in shots per second.")]
    public float fireRate = 5f;
    [Tooltip("Damage dealt by the weapon per shot.")]
    public int damage = 10;
    public ParticleSystem muzzleFlash;
}