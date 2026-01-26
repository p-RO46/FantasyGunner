using UnityEngine;

[CreateAssetMenu(fileName = "ArmorItemData", menuName = "Scriptable Objects/ItemData/ArmorItemData")]
public class ArmorItemData : ItemData
{
    public int armorAmount = 50;
    public override void Use()
    {
        // 플레이어 체력 회복 로직 호출
        Debug.Log($"체력이 {armorAmount}만큼 추가되었습니다.");
    }
}
