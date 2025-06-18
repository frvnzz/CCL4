using UnityEngine;

public class AmmoBoxPickup : MonoBehaviour
{
    [Tooltip("Assign the gun prefab this ammo box is for.")]
    public GameObject weaponPrefab; // The weapon this ammo box is for

    public int ammoAmount = 30; // How much ammo to give

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            WeaponManager weaponManager = other.GetComponentInParent<WeaponManager>();
            weaponManager.AddAmmo(weaponPrefab, ammoAmount);
            Destroy(gameObject); // Remove the ammo box after pickup
        }
    }
}
