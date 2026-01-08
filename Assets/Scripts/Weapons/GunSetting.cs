using UnityEngine;

public class GunSetting : MonoBehaviour
{
    public WeaponData weaponData;
    
    void SetModel()
    {
        GetComponent<MeshRenderer>().material = weaponData.material;
        GetComponent<MeshFilter>().mesh = weaponData.mesh;
    }

    public void SetWeaponData(WeaponData w)
    {
        weaponData = w;
        SetModel();
    }
}
