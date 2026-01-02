using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Stats")]
    public string gunName;
    public float damage;

    [Header("Shooting Stats")]
    public float recoilForce;
    public float fireRate;

    [Header("Ammo Settings")]
    public int maxAmmo;
    public float reloadTime;
}
