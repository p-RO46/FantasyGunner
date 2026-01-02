using UnityEngine;

public class GunHandler : MonoBehaviour
{
    public WeaponData currentWeaponData;

    void Shoot()
    {
        // 데이터 파일에 저장된 값을 가져와서 사용합니다.
        Debug.Log(currentWeaponData.gunName + " 발사!");
        Debug.Log("데미지: " + currentWeaponData.damage);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
