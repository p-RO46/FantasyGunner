using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int _scraps;
    [SerializeField]
    private List<WeaponData> _ownWeapons;
    [SerializeField]
    private List<Ammos> _ownAmmos;
    [SerializeField]
    private List<Items> _ownItems;

    public TextMeshProUGUI scrapText;

    void UpdateScrap()
    {
        scrapText.text = "Scrap: $" + _scraps;
    }

    public int GetScrap()
    {
        return _scraps;
    }

    public void AddScrap(int amount)
    {
        _scraps += amount;
        UpdateScrap();
    }

    #region AddItems
    public bool AddWeapon(WeaponData weapon)
    {
        bool hasWeapon = _ownWeapons.Exists(x => x.gunName.Equals(weapon.gunName));
        if (!hasWeapon)
        {
            _ownWeapons.Add(weapon);
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
    }
    #endregion

    #region UseItems
    public void UseScrap(int amount) // 스크랩 사용
    {
        _scraps -= amount;
        UpdateScrap();
    }

    public void LostWeapon(WeaponData weapon) // 총 버림
    {
        int index = _ownWeapons.FindIndex(x => x.gunName.Equals(weapon.gunName));
        if (index != -1) // 존재하는 경우
        {
            _ownWeapons.RemoveAt(index);
        }
        else // 없는 경우
        {
            Debug.Log("LostWeapon : 해당하는 총이 없습니다.");
        }
    }

    public void UseAmmos(AmmoData ammo, int count) // 총알 소모
    {
        int index = _ownAmmos.FindIndex(x => x.ammo.ammoName.Equals(ammo.ammoName));
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
    public void UseItems(ItemData item, int count)
    {
        int index = _ownItems.FindIndex(x => x.item.itemName.Equals(item.itemName));
        if (index != -1) // 존재하는 경우
        {
            item.Use();
            _ownItems[index].count -= count;
        }
        else // 없는 경우
        {
            Debug.Log("LostItem : 해당하는 아이템이 없습니다.");
        }
    }
    #endregion
}
