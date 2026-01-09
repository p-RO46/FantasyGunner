using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Stats")]
    public string CharacterName;
    public string CharacterType;
    public float Speed;
    public int Damage;
    public int MaxHP;
    public float DeadTime = 1f;

    [Header("Attack Stats")]
    public GameObject AttackObject;
    public float AttackPower;
    public float AttackTime;
    public bool HasStun;
    public float StunTime;
    public float AttackRange; //플레이어 감지 거리
}
