using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private PlayerInventory pi;

    public GameObject WeaponListUI;
    public GameObject ItemListUI;

    public Sprite trans;
    private Image image;


    private void Start()
    {
        pi = GameObject.Find("GameManager").transform.GetChild(0).gameObject.GetComponent<PlayerInventory>();
        pi.Notice_InvetoryUI(this);

        UpdateWeaponUI();
        UpdateItemUI();
    }

    public void UpdateWeaponUI()
    {
        int index = 0;
        WeaponData weapon;
        foreach(Transform w in WeaponListUI.transform)
        {
            weapon = pi.GetWeapons(index++);

            if (weapon != null)
            {
                image = w.GetChild(0).gameObject.GetComponent<Image>();
                image.sprite = weapon.icon;
            }
        }
    }
    public void UpdateItemUI()
    {
        int index = 0;
        Items item;
        TextMeshProUGUI count;
        foreach (Transform w in ItemListUI.transform)
        {
            item = pi.GetItems(index++);

            image = w.GetChild(0).gameObject.GetComponent<Image>();
            count = w.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            if (item != null)
            {
                image.sprite = item.item.icon;
                count.text = item.count.ToString();
            }
            else
            {
                image.sprite = trans;
                count.text = "0";
            }
        }
    }
}
