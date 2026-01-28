using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int _scraps = 0;
    [SerializeField]
    private List<WeaponData> _ownWeapons;
    [SerializeField]
    private List<Ammos> _ownAmmos;
    [SerializeField]
    private List<Items> _ownItems;
    [SerializeField]
    private Items _ownKeys;

    public InventoryManager im; // 플레이어가 들고 있는 무기를 swap 하는 코드... 엉망진창
    // 애초에 종류별로 클래스를 하나씩 만들었어야 하나? 싶기도

    public TextMeshProUGUI scrapText;
    public TextMeshProUGUI keyText;
    public InventoryUI inventoryUI;

    public ItemData key; // 열쇠 데이터는 특별하게 다룬다...

    public int w_size;
    public int a_size;
    public int i_size;

    private List<Weapon> _observers;

    private void Start()
    {
        _ownKeys = new Items(key, 0);
        _observers = new List<Weapon>();

        w_size = _ownWeapons.Count;
        a_size = _ownAmmos.Count;
        i_size = _ownItems.Count;

        UpdateScrap();
    }

    #region Observer
    public void AddObserver(Weapon w)
    {
        _observers.Add(w);
        AddAmmos(w.weaponData.fittedAmmo, w.remainBullet);
    }
    void noticeToWeapons()
    {
        // Bullet의 사용 현황을 Weapon 객체들에게 알리는 함수
        // 같은 총이 두 자루일 수는 없는데?
        foreach (Weapon w in _observers)
        {
            if (w.weaponData.gunName != null)
            {
                w.remainBullet = GetAmmos(w.weaponData.fittedAmmo).count;
            }
        }
        im.UpdateRemainAmmos();
    }
    #endregion

    #region Key
    void UpdateKey()
    {
        keyText.text = _ownKeys.count.ToString();
    }
    public void AddKey()
    {
        _ownKeys.count++;
        UpdateKey();
    }
    public void AddKeys(ItemData item, int count)
    {
        _ownKeys.count += count;
        UpdateKey();
    }
    public void UseKey()
    {
        _ownKeys.count--;
        UpdateKey();
    }
    public bool ExistKey()
    {
        if(_ownKeys.count > 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Scrap
    void UpdateScrap()
    {
        scrapText.text = _scraps.ToString();
    }

    public float GetScrap()
    {
        return _scraps;
    }

    public void AddScrap(int amount)
    {
        _scraps += amount;
        UpdateScrap();
    }
    #endregion

    #region GetItems
    public WeaponData GetWeapons(int index)
    {
        if(index < _ownWeapons.Count && index >= 0)
        {
            return _ownWeapons[index];
        }

        return null;
    }

    public Ammos GetAmmos(AmmoData ammo)
    {
        int index = _ownAmmos.FindIndex(x => x.ammo.Equals(ammo));
        if (index != -1) // 존재하는 경우
        {
            return _ownAmmos[index];
        }

        return null;
    }

    public Items GetItems(int index)
    {
        if (index < _ownItems.Count && index >= 0)
        {
            return _ownItems[index];
        }

        return null;
    }
    #endregion

    #region AddItems
    public bool AddWeapon(WeaponData weapon)
    {
        bool hasWeapon = _ownWeapons.Exists(x => x.gunName.Equals(weapon.gunName));
        if (!hasWeapon)
        {
            _ownWeapons.Add(weapon);
            im.AddWeapon(weapon);
            inventoryUI.UpdateWeaponUI();
            w_size = _ownWeapons.Count;
            return true;
        }
        else
        {
            Debug.Log("AddWeapon : 이미 가지고 있는 총입니다.");
            return false;
        }
    }

    public void AddAmmos(AmmoData ammo, int count)
    {
        int index = _ownAmmos.FindIndex(x => x.ammo.ammoName.Equals(ammo.ammoName));
        if (index != -1) // 이미 존재하는 경우
        {
            _ownAmmos[index].count += count;
        }
        else // 없는 경우
        {
            _ownAmmos.Add(new Ammos(ammo, count));
            a_size = _ownAmmos.Count;
        }
        noticeToWeapons();
    }

    public void AddItems(ItemData item,int count)
    {
        int index = _ownItems.FindIndex(x => x.item.itemName.Equals(item.itemName));
        if (index != -1) // 이미 존재하는 경우
        {
            _ownItems[index].count += count;
        }
        else // 없는 경우
        {
            _ownItems.Add(new Items(item, count));
            i_size = _ownItems.Count;
        }
        inventoryUI.UpdateItemUI();
    }
    #endregion

    #region UseItems
    public void UseScrap(int amount) // 스크랩 사용
    {
        _scraps -= amount;
        UpdateScrap();
    }

    public void LostWeapon(string type) // 총 버림
    {
        int index = _ownWeapons.FindIndex(x => x.gunName.Equals(type));
        if (index != -1) // 존재하는 경우
        {
            _ownWeapons.RemoveAt(index);
            inventoryUI.UpdateWeaponUI();
            w_size = _ownWeapons.Count;
        }
        else // 없는 경우
        {
            Debug.Log("LostWeapon : 해당하는 총이 없습니다.");
        }
    }

    public void UseAmmos(string type, int count) // 총알 소모(장전)
    {
        int index = _ownAmmos.FindIndex(x => x.ammo.ammoName.Equals(type));
        if (index != -1) // 존재하는 경우
        {
            if (_ownAmmos[index].count >= count)
            {
                _ownAmmos[index].count -= count;
                noticeToWeapons();
            }
            else
            {
                Debug.Log("UseAmmos : 총알이 부족합니다. 장전 실패");
            }
        }
        else // 없는 경우
        {
            Debug.Log("UseAmmo : 해당하는 총알이 없습니다.");
        }
    }

    // 아이템 사용
    public void UseItems(string type, int count)
    {
        int index = _ownItems.FindIndex(x => x.item.itemName.Equals(type));
        if (index != -1) // 존재하는 경우
        {
            _ownItems[index].item.Use();
            _ownItems[index].count -= count;

            if (_ownItems[index].count <= 0)
            {
                _ownItems.RemoveAt(index);
                i_size = _ownItems.Count;
            }

            inventoryUI.UpdateItemUI();
        }
        else // 없는 경우
        {
            Debug.Log("UseItem : 해당하는 아이템이 없습니다.");
        }
    }
    #endregion

    #region ExistItems
    public bool ExistItems(string type)
    {
        int index = _ownItems.FindIndex(x => x.item.itemName.Equals(type));
        return index != -1 && _ownItems[index].count > 0;
    }
    #endregion
}
