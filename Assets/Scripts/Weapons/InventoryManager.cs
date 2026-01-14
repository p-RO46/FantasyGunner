using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponData> weapondatas;
    public Weapon currentWeapon;
    public GameObject weapon;
    public List<Weapon> weaponInventory;
    public bool isSwapping = false;
    public Coroutine swapCoroutine;

    private WeaponHandler handler;
    private bool initializer = true; //최초 초기화용 인자. 초기화 완료 후엔 false

    void Start()
    {
        init();
    }
    void init()
    {
        handler = GetComponent<WeaponHandler>();
        weaponInventory = new List<Weapon>();
        foreach(WeaponData w in weapondatas)
        {
            weaponInventory.Add(new Weapon(w));
        }
        SwapWeapon(1); // default:pistol
        initializer = false;
    }

    // 1:pistol 2:smg 3:ar 4:sg 5:sr
    public void SwapWeapon(int n)
    {
        if (handler.isShooting)
        {
            Debug.Log("총 발사중엔 교체 불가입니다.");
            return;
        }

        if(weaponInventory.Count >= n && n > 0)
        {
            if(!initializer && weaponInventory[n - 1].weaponData.gunName.Equals(currentWeapon.weaponData.gunName))
            {
                Debug.Log("이미 장착 중인 총입니다. 교체를 취소합니다.");
                return;
            }

            if (handler.isReloading)
            {
                handler.isReloading = false;
                //StopCoroutine(handler.reloadCoroutine);
                Debug.Log("장전 취소");
            }

            if (isSwapping)
            {
                StopCoroutine(swapCoroutine);
                isSwapping = false;
                Debug.Log("교체 취소");
            }

            isSwapping = true;
            if (weaponInventory.Count >= n && n > 0)
            {
                currentWeapon = weaponInventory[n - 1];
                Debug.Log("총 교체 시작합니다.");
                handler.EquipGun(currentWeapon);
                weapon.GetComponent<GunSetting>().SetWeaponData(currentWeapon.weaponData);
                if (!handler.isReloadingbySwaping)
                {
                    swapCoroutine = StartCoroutine(waitForEndOfSwap());
                }
            }
        }
        else
        {
            Debug.Log("인자가 인벤토리 범위를 벗어납니다. 교체 불가");
        }
    }

    public void SwapWeapon(InputAction.CallbackContext context)
    {
        int keyPressed;
        if (context.started && int.TryParse(context.control.name, out keyPressed))
        {
            SwapWeapon(keyPressed);
        }
    }

    private IEnumerator waitForEndOfSwap()
    {
        weapon.GetComponent<Animator>().SetTrigger("Equip");
        yield return Util.WaitForSecond(currentWeapon.weaponData.equipTime);
        isSwapping = false;
        Debug.Log("교체 끝");
    }
}
