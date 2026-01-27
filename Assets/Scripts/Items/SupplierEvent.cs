using UnityEngine;

public class SupplierEvent : MonoBehaviour
{
    private WeaponData WeaponBuy;
    private AmmoData AmmoBuy;
    private ItemData ItemBuy;
    public int count = 1;

    public GameObject StoreUI;

    private PlayerInventory pi;
    private ItemAll itemAll;

    private bool isOpened = false; // Store UI

    private void Start()
    {
        pi = GetComponent<PlayerInventory>();
        itemAll = GetComponent<ItemAll>();
    }

    public void OpenStore()
    {
        isOpened = true; // 상태 반전
        StoreUI.SetActive(isOpened); // 상태에 따라 켜거나 끔

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // 필요하다면 게임 일시정지
        Time.timeScale = 0f;
    }

    public void ExitStore()
    {
        isOpened = false; // 상태 반전
        StoreUI.SetActive(isOpened); // 상태에 따라 켜거나 끔

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // 게임 다시 시작
        Time.timeScale = 1f;
    }

    #region BuyWeapons
    public void BuyPistol()
    {
        WeaponBuy = itemAll.AllWeapons[0];
        BuyWeapons();
    }
    public void BuySMG()
    {
        WeaponBuy = itemAll.AllWeapons[1];
        BuyWeapons();
    }
    public void BuyAR()
    {
        WeaponBuy = itemAll.AllWeapons[2];
        BuyWeapons();
    }
    public void BuySG()
    {
        WeaponBuy = itemAll.AllWeapons[3];
        BuyWeapons();
    }
    public void BuySR()
    {
        WeaponBuy = itemAll.AllWeapons[4];
        BuyWeapons();
    }

    void BuyWeapons()
    {
        if (pi.GetScrap() >= WeaponBuy.scrapPrice * count)
        {
            if (pi.AddWeapon(WeaponBuy))
            {
                pi.UseScrap(WeaponBuy.scrapPrice * count);
            }
        }
        else
        {
            Debug.Log("스크랩 부족. 구매 불가");
        }
    }
    #endregion

    #region BuyAmmos
    public void BuyPistolAmmos()
    {
        AmmoBuy = itemAll.AllAmmos[0];
        BuyAmmos();
    }
    public void BuySMGAmmos()
    {
        AmmoBuy = itemAll.AllAmmos[1];
        BuyAmmos();
    }
    public void BuyARAmmos()
    {
        AmmoBuy = itemAll.AllAmmos[2];
        BuyAmmos();
    }
    public void BuySGAmmos()
    {
        AmmoBuy = itemAll.AllAmmos[3];
        BuyAmmos();
    }
    public void BuySRAmmos()
    {
        AmmoBuy = itemAll.AllAmmos[4];
        BuyAmmos();
    }

    void BuyAmmos()
    {
        if (pi.GetScrap() >= AmmoBuy.scrapPrice * count)
        {
            pi.UseScrap(AmmoBuy.scrapPrice * count);
            pi.AddAmmos(AmmoBuy, count);
        }
        else
        {
            Debug.Log("스크랩 부족. 구매 불가");
        }
    }
    #endregion

    #region BuyItems
    public void BuyHealKit()
    {
        ItemBuy = itemAll.AllItems[0];
        BuyItems();
    }
    public void BuyArmor()
    {
        ItemBuy = itemAll.AllItems[1];
        BuyItems();
    }

    void BuyItems()
    {
        if (pi.GetScrap() >= ItemBuy.scrapPrice * count)
        {
            pi.UseScrap(ItemBuy.scrapPrice * count);
            pi.AddItems(ItemBuy, count);
        }
        else
        {
            Debug.Log("스크랩 부족. 구매 불가");
        }
    }
    #endregion
}
