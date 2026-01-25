using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeItemData", menuName = "Scriptable Objects/ItemData/GrenadeItemData")]
public class GrenadeItemData : ItemData
{
    public int damage = 50;
    public override void Use()
    {
        // 플레이어 수류탄 던지는 함수 호출하기
        Debug.Log($"데미지를 {damage}만큼 입혔습니다.");
    }
}
    