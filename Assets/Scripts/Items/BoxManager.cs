using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public BoxRaycast boxRaycast;
    private GameObject GameUI;
    private GameObject BoxUI;

    private GameObject supplyManager;
    private PlayerInventory pi;

    private Animator anim;
    private bool isOpen = false;

    public ItemData item; // box에 들어있는 아이템. 랜덤 배정? 일단 key로 고정
    private AmmoData ammo;
    private WeaponData weapon;

    private bool canOpen = false;
    private int count = 1;

    private void Start()
    {
        anim = GetComponent<Animator>();
        supplyManager = GameObject.Find("GameManager").transform.GetChild(2).gameObject;
        pi = supplyManager.GetComponent<PlayerInventory>();

        GameUI = GameObject.Find("GameUIParent").transform.GetChild(0).gameObject;
        BoxUI = GameUI.transform.Find("BoxOpen UI").gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            boxRaycast.nearPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            boxRaycast.nearPlayer = false;
        }
    }
    public void CanOpen(bool b)
    {
        if (!isOpen)
        {
            BoxUI.SetActive(b);
            canOpen = b;
        }
    }

    public void BoxOpen()
    {
        if (canOpen && !isOpen)
        {
            isOpen = true;
            anim.SetBool("isOpen", true);
            StartCoroutine(DisappearItem());
            GetKey(item);
        }
    }

    void SetItem() //들어있는 아이템 랜덤 배정
    {
        // 근데 일단 테스트로 키만 넣어놓겠심
    }

    void GetKey(ItemData i)
    {
        pi.AddKey();
    }

    void GetItem(ItemData i)
    {
        pi.AddItems(i, count);
    }
    void GetItem(AmmoData a)
    {
        pi.AddAmmos(a, count);
    }
    void GetItem(WeaponData w)
    {
        pi.AddWeapon(w);
    }

    IEnumerator DisappearItem()
    {
        yield return Util.WaitForSecond(3f);
        Destroy(transform.GetChild(1).gameObject);
    }
}
