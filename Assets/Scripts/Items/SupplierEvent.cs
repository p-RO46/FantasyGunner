using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SupplierEvent : MonoBehaviour
{
    public InputActionAsset ipa;

    private WeaponData WeaponBuy;
    private AmmoData AmmoBuy;
    private ItemData ItemBuy;
    public int count = 1;

    private GameObject StoreUI;
    private PlayerInventory pi;
    private ItemAll itemAll;

    private bool isOpened = false; // Store UI
    private InputAction _storeAction;
    private Button myButton;

    Transform Ammo1;
    Transform Ammo2;
    Transform Guns1;
    Transform Guns2;
    Transform Items;

    private void Start()
    {
        pi = GetComponent<PlayerInventory>();
        itemAll = GetComponent<ItemAll>();
    }

    public void OpenStore(InputAction.CallbackContext context)
    {
        // 인벤토리를 열 때
        ipa.FindActionMap("GamePlay").Disable();
        ipa.FindActionMap("UI").Enable();

        isOpened = true;
        StoreUI.GetComponent<Canvas>().enabled = isOpened;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // 필요하다면 게임 일시정지
        Time.timeScale = 0f;
    }

    public void ExitStore()
    {
        // 인벤토리를 닫을 때
        ipa.FindActionMap("UI").Disable();
        ipa.FindActionMap("GamePlay").Enable();

        isOpened = false; // 상태 반전
        StoreUI.GetComponent<Canvas>().enabled = isOpened;

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

    void OnEnable()
    {
        // 에셋 내의 "Player" 액션 맵에서 "Move" 액션을 찾음
        var playerMap = ipa.FindActionMap("GamePlay");
        _storeAction = playerMap.FindAction("Store UI");

        _storeAction.started += OpenStore;
        _storeAction.Enable();
    }
    void OnDisable()
    {
        _storeAction.started -= OpenStore;
        _storeAction.Disable();

        //이 시점에서 어차피 UI가 파괴되었기 때문에 일일이 이벤트를 해제해줄 필요는 없다.
        /*
        myButton.onClick.RemoveListener(ExitStore);

        #region Crazy Hard Coding Button
        Ammo1.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(BuyPistolAmmos);
        Ammo1.GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveListener(BuySMGAmmos);
        Ammo1.GetChild(2).gameObject.GetComponent<Button>().onClick.RemoveListener(BuyARAmmos);
        Ammo1.GetChild(3).gameObject.GetComponent<Button>().onClick.RemoveListener(BuySGAmmos);
        Ammo2.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(BuySRAmmos);

        Guns1.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(BuyPistol);
        Guns1.GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveListener(BuySMG);
        Guns1.GetChild(2).gameObject.GetComponent<Button>().onClick.RemoveListener(BuyAR);
        Guns1.GetChild(3).gameObject.GetComponent<Button>().onClick.RemoveListener(BuySG);
        Guns2.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(BuySR);

        Items.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(BuyHealKit);
        Items.GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveListener(BuyArmor);
        #endregion
        */
    }
    public void Notice_StoreUI(GameObject storeUI)
    {
        StoreUI = storeUI;
        //Debug.Log("Store UI를 찾았습니다");

        #region Crazy Hard Coding Button
        Ammo1 = StoreUI.transform.Find("Store/Scroll View/Viewport/Content/Supplies_Ammo1");
        Ammo2 = StoreUI.transform.Find("Store/Scroll View/Viewport/Content/Supplies_Ammo2");
        Guns1 = StoreUI.transform.Find("Store/Scroll View/Viewport/Content/Supplies_Guns1");
        Guns2 = StoreUI.transform.Find("Store/Scroll View/Viewport/Content/Supplies_Guns2");
        Items = StoreUI.transform.Find("Store/Scroll View/Viewport/Content/Supplies_Items");

        Ammo1.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(BuyPistolAmmos);
        Ammo1.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(BuySMGAmmos);
        Ammo1.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(BuyARAmmos);
        Ammo1.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(BuySGAmmos);
        Ammo2.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(BuySRAmmos);

        Guns1.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(BuyPistol);
        Guns1.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(BuySMG);
        Guns1.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(BuyAR);
        Guns1.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(BuySG);
        Guns2.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(BuySR);

        Items.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(BuyHealKit);
        Items.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(BuyArmor);
        #endregion
    }

    public void Notice_ExitButton(Button button)
    {
        myButton = button;
        myButton.onClick.AddListener(ExitStore);
    }
}
