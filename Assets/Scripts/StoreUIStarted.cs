using UnityEngine;

public class StoreUIStarted : MonoBehaviour
{
    void Start()
    {
        SupplierEvent supplierEvent = GameObject.Find("GameManager").transform.Find("SupplyManager").gameObject.GetComponent<SupplierEvent>();
        supplierEvent.Notice_StoreUI(this.gameObject);
    }
}
