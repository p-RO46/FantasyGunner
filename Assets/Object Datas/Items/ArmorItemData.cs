using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItemData", menuName = "Scriptable Objects/ItemData/ArmorItemData")]
public class ArmorItemData : ItemData
{
    public int armorAmount = 50;
    public override void Use()
    {
        PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerController.Armored(armorAmount);
        // 최대 체력을 추가하는? 추가 체력 로직 필요
        // Armor 파괴 전에 한번 더 쓰면 그냥 충전 (최대보다는 안올라감)
    }
}
