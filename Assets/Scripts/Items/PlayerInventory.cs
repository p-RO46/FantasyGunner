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

    public TextMeshProUGUI scrapText;
    public TextMeshProUGUI keyText;
    public InventoryUI inventoryUI;

    public ItemData key; // 열쇠 데이터는 특별하게 다룬다...

    private void Start()
    {
        _ownKeys = new Items(key, 0);
        UpdateScrap();
    }

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

    public Ammos GetAmmos(string type)
    {
        int index = _ownAmmos.FindIndex(x => x.ammo.ammoName.Equals(type));
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
            inventoryUI.UpdateWeaponUI();
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
        }
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
        }
        else // 없는 경우
        {
            Debug.Log("LostWeapon : 해당하는 총이 없습니다.");
        }
    }

    public void UseAmmos(string type, int count) // 총알 소모
    {
        int index = _ownAmmos.FindIndex(x => x.ammo.ammoName.Equals(type));
        if (index != -1) // 존재하는 경우
        {
            _ownAmmos[index].count -= count;
        }
        else // 없는 경우
        {
            Debug.Log("LostAmmo : 해당하는 총알이 없습니다.");
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
        }
        else // 없는 경우
        {
            Debug.Log("LostItem : 해당하는 아이템이 없습니다.");
        }
    }
    #endregion

    #region ExistItems
    public bool ExistItems(string type)
    {
        int index = _ownItems.FindIndex(x => x.item.itemName.Equals(type));
        if (index != -1 && _ownItems[index].count > 0) // 존재하는 경우
        {
            return true;
        }
        else // 없는 경우
        {
            return false;
        }
    }
    #endregion
}
