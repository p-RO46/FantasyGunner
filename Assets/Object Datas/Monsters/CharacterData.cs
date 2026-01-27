using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Stats")]
    public string CharacterName;
    public string CharacterType;
    public float Speed = 1f;
    public float RunSpeed = 2f;
    public float RotationSpeed;
    public int Damage;
    public int MaxHP;
    public float DeadTime = 1f;
    public float WalkInterval = 4f;

    [Header("Attack Stats")]
    public GameObject AttackObject;
    public float AttackPower;
    public float AttackTime;
    public bool HasStun;
    public float StunTime;
    public float AttackRange = 5f; // 공격 가능 거리
    public float DetectRange = 20f; // 플레이어 감지 거리 (시야 범위)
    public float DetectAngle = 90f; // 시야각

    [Header("Loot Stats")]
    public int DropCredits = 10;

}
