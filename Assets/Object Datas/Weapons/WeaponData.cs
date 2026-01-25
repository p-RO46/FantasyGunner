using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    /// <summary>
    /// equipTime은 항상 reloadTime 보다 짧거나 같아야 합니다.
    /// 그렇지 않으면 큰일? 납니다
    /// </summary>
    [Header("Basic Stats")]
    public string gunName;
    public int damage;
    public float equipTime;
    public int scrapPrice;
    public Sprite icon;

    [Header("Shooting Stats")]
    public bool canContinuousFire;
    public bool hasScope;
    public float verticalForce;
    public float horizontalForce;
    public float accuracy;
    public float fireRate;
    public bool hasStun;
    public float stunTime;

    [Header("Ammo Settings")]
    public AmmoData fittedAmmo;
    public int maxAmmo;
    public int loadAmmo;
    public float reloadTime;

    [Header("Model Settings")]
    public Mesh mesh;
    public Material material;
}
