using TMPro;
using UnityEngine;

public class KeyStarted : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInventory pi = GameObject.Find("GameManager").transform.Find("SupplyManager").GetComponent<PlayerInventory>();
        pi.Notice_KeyText(GetComponent<TextMeshProUGUI>());
    }

}
