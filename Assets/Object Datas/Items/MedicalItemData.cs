using UnityEngine;

[CreateAssetMenu(fileName = "MedicalItemData", menuName = "Scriptable Objects/ItemData/MedicalItemData")]
public class MedicalItemData : ItemData
{
    public int healAmount = 50;
    public override void Use()
    {
        // 플레이어 체력 회복 로직 호출
        Debug.Log($"체력을 {healAmount}만큼 회복했습니다.");
    }
}