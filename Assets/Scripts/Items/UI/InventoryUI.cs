using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory pi;

    public GameObject WeaponListUI;
    public GameObject ItemListUI;

    private Image image;

    private void Start()
    {
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

            if (item != null)
            {
                image = w.GetChild(0).gameObject.GetComponent<Image>();
                image.sprite = item.item.icon;

                count = w.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                count.text = item.count.ToString();
            }
        }
    }
}
