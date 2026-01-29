using UnityEngine;
using UnityEngine.UI;

public class ExitButtonStarted : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SupplierEvent supplierEvent = GameObject.Find("GameManager").transform.Find("SupplyManager").gameObject.GetComponent<SupplierEvent>();
        supplierEvent.Notice_ExitButton(GetComponent<Button>());
    }
}
