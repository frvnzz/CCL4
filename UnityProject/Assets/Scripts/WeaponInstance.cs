using UnityEngine;

[System.Serializable]
public class WeaponInstance
{
    public GameObject Prefab { get; private set; }
    public GunStats Stats { get; set; }
    public GameObject GunObject { get; set; }
    public int CurrentAmmo { get; set; }
    public int TotalAmmo { get; set; }

    public WeaponInstance(GameObject prefab, GunStats stats)
    {
        Prefab = prefab;
        Stats = stats;
        CurrentAmmo = stats != null ? stats.maxAmmo : 10;
        TotalAmmo = stats != null ? stats.startingTotalAmmo : 90;
    }

    public void ResetAmmo()
    {
        if (Stats != null)
        {
            CurrentAmmo = Stats.maxAmmo;
            TotalAmmo = Stats.startingTotalAmmo;
        }
    }
}